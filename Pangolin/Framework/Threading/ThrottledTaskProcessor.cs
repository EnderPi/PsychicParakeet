using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace EnderPi.Framework.Threading
{
    /// <summary>
    /// General throttling kind of processor.  Tell it how to get a task, and the delegate to call with the task, and it manages 
    /// scheduling and running up to N amount of those tasks at a time.  It sleeps when there is no work, and works up to max
    /// parallelism when tasks are available.  Has configurations for polling frequency, paralellism, and supports
    /// terminating hung tasks.  Provides event hooks for customizing behavior.
    /// </summary>
    /// <remarks>
    /// This is intended to be used as a singleton in a process.  You create one to do something, start it, and let it go.
    /// It has many safety rails to preserve good state while running.  You might want to stop it, change some things, 
    /// then re-start it.  Typically, the delegate pulls a task off a message queue, then schedules it to run.
    /// Can be made to force task retry on some sort of schedule by handling the appropriate events.
    /// </remarks>
    /// <typeparam name="T">The class that encapsulates a task</typeparam>
    public class ThrottledTaskProcessor<T> where T : class
    {

        #region Private Fields

        /// <summary>
        /// The semaphore that manages the thread count.
        /// </summary>
        private SemaphoreSlim _semaphore;

        /// <summary>
        /// Internal cancellation token source.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Internal token for cancellation.
        /// </summary>
        private CancellationToken _token;

        /// <summary>
        /// The processor to call on the task.
        /// </summary>
        private Action<T> _processor;

        /// <summary>
        /// How long a task is allowed to run before it is terminated.
        /// </summary>
        private int _maxTaskLifetimeinSeconds;

        /// <summary>
        /// The milliseconds to sleep when there are no tasks to do.
        /// </summary>
        private int _millisecondsToSleep;

        /// <summary>
        /// Internal list of tasks that are currently processing.  Used for cancellation purposes.
        /// </summary>
        private List<CancellableTask> _currentTasks;

        /// <summary>
        /// Object for synchronizing access to the task list.
        /// </summary>
        private object _lockObject = new object();

        /// <summary>
        /// Timer to fire periodic cleanup events.
        /// </summary>
        private Timer _taskKillingTimer;

        /// <summary>
        /// The delegate that gets a task.
        /// </summary>
        private Func<T> _getTask;

        /// <summary>
        /// Handle for sleeping so that cancellation can be timely.
        /// </summary>
        private ManualResetEventSlim _cancellationEvent;

        /// <summary>
        /// Maximum number of worker threads.
        /// </summary>
        private int _maximumConcurrency;

        /// <summary>
        /// True if the processor is running, false otherwise.
        /// </summary>
        private bool _running;

        /// <summary>
        /// Seconds between housekeeping events
        /// </summary>
        private int _secondsBetweenHousekeeping;

        #endregion

        /// <summary>
        /// Sets or gets the maximum number of worker threads.  Can't be set if the processor is running.
        /// </summary>
        public int MaximumWorkerThreads
        {
            set
            {
                if (_running)
                {
                    throw new FieldAccessException("Cannot set maximum threads while processor is running");
                }
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Maximum number of worker threads cannot be less than 1");
                }
                _maximumConcurrency = value;
            }
            get 
            {
                return _maximumConcurrency; 
            }
        }

        public int MillisecondsToSleep
        {
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Milliseconds to sleep cannot be less than 1.");
                }
                Interlocked.Exchange(ref _millisecondsToSleep, value);
            }
            get
            {
                return _millisecondsToSleep;
            }
        }

        public int MaxTaskLifeTimeInSeconds
        {
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Max task life time in seconds cannot be less than 1.");
                }
                Interlocked.Exchange(ref _maxTaskLifetimeinSeconds, value);
            }
            get
            {
                return _maxTaskLifetimeinSeconds;
            }
        }

        public int SecondsBetweenHousekeepingEvents
        {
            set
            {
                if (_running)
                {
                    throw new FieldAccessException("Cannot set seconds between housekeeping events while processor is running");
                }
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Seconds between housekeeping events cannot be less than 1");
                }
                _secondsBetweenHousekeeping = value;
            }
            get
            {
                return _secondsBetweenHousekeeping;
            }
        }


        #region Events

        //events tied to this processor.
        public delegate void ThrottledEventHandler(object sender, ThrottledEventArgs<T> e);

        /// <summary>
        /// Fires just before a task is passed to the processing delegate. 
        /// </summary>
        public event ThrottledEventHandler MessageProcessing;

        /// <summary>
        /// Fires after a task is processed by the processing delegate.
        /// </summary>
        public event ThrottledEventHandler MessageProcessed;

        /// <summary>
        /// Fires when an exception comes out of the processor delegate.
        /// </summary>
        public event ThrottledEventHandler ExceptionOccurredInWorkerThread;

        /// <summary>
        /// Fires when an exception occurs in the scheduling thread.  Typically indicates an environmental issue.
        /// </summary>
        public event ThrottledEventHandler ExceptionOccurredInSchedulerThread;

        /// <summary>
        /// Occurs after periodic housekeeping has been performed (terminating hung tasks.)
        /// </summary>
        public event ThrottledEventHandler CleanupTaskFired;
        
        /// <summary>
        /// Occurs after all tasks have stopped and all cleanup has occurred.
        /// </summary>
        public event ThrottledEventHandler StoppedListening;

        private void OnExceptionOccurredInWorkerThread(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => ExceptionOccurredInWorkerThread?.Invoke(this, e));
        }

        private void OnExceptionOccurredInSchedulerThread(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => ExceptionOccurredInSchedulerThread?.Invoke(this, e));
        }

        private void OnMessageProcessing(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => MessageProcessing?.Invoke(this, e));
        }

        private void OnMessageProcessed(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => MessageProcessed?.Invoke(this, e));
        }

        private void OnCleanupTaskFired(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => CleanupTaskFired?.Invoke(this, e));
        }

        private void OnStoppedListeningEventFired(ThrottledEventArgs<T> e)
        {
            Threading.ExecuteWithoutThrowing(() => StoppedListening?.Invoke(this, e));
        }

        #endregion

        /// <summary>
        /// Constructor with some default parameters
        /// </summary>
        /// <param name="GetTask">The function to get a task.  Should return null if no task exists.</param>
        /// <param name="processor">The processor to call with a task.</param>
        public ThrottledTaskProcessor(Func<T> GetTask, Action<T> processor) : this(GetTask, processor, 4, 120, 4000, 30)    
        {   
        }

        public ThrottledTaskProcessor(Func<T> GetTask, Action<T> processor, int concurrency, int maxtaskLifetimeInSeconds, int millisecondsToSleep, int secondsBetweenHousekeeping) 
        {
            _processor = processor;
            _getTask = GetTask;
            _currentTasks = new List<CancellableTask>();
            _maximumConcurrency = concurrency;
            _maxTaskLifetimeinSeconds = maxtaskLifetimeInSeconds;
            _millisecondsToSleep = millisecondsToSleep;
            _secondsBetweenHousekeeping = secondsBetweenHousekeeping;
        }
        
        /// <summary>
        /// Starts listening for tasks and processing them.
        /// </summary>
        /// <remarks>
        /// This method does a significant amount of state intiialization.
        /// </remarks>
        public void StartListening()
        {
            _running = true;
            _semaphore = new SemaphoreSlim(_maximumConcurrency);
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            _token.Register(()=>_cancellationEvent.Set());
            _cancellationEvent = new ManualResetEventSlim();
            Thread thread = new Thread(() => SchedulerThread())
            {
                IsBackground = true
            };
            thread.Start();
            _taskKillingTimer = new Timer(_secondsBetweenHousekeeping * 1000);
            _taskKillingTimer.Elapsed += CleanupHungTasks;
            _taskKillingTimer.AutoReset = true;
            _taskKillingTimer.Enabled = true;
        }

        /// <summary>
        /// Private delegate that does cleanup - terminating hung tasks, and fires an event.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CleanupHungTasks(object source, ElapsedEventArgs e)
        {
            lock (_lockObject)
            {
                DateTime now = DateTime.Now;
                foreach (var task in _currentTasks)
                {
                    if ((now - task.TimeStarted).Seconds > _maxTaskLifetimeinSeconds)
                    {
                        task.Cancel();
                    }
                }
            }
            OnCleanupTaskFired(new ThrottledEventArgs<T>());
        }

        /// <summary>
        /// The main delegate that gets tasks and schedules them.
        /// </summary>
        private void SchedulerThread()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    _semaphore.Wait(_token);
                    if (_token.IsCancellationRequested)     //That semaphore blocking call may have returned due to cancellation.
                    {
                        return;
                    }
                    var message = _getTask();
                    if (message != null)
                    {
                        CancellableTask task = new CancellableTask();
                        task.LifeTimeInSeconds = _maxTaskLifetimeinSeconds;     //Eventually this may be set through an interface
                        task.SetAction(() => ProcessAsync(message, task));
                        lock (_lockObject)
                        {
                            _currentTasks.Add(task);
                            task.Start();
                        }
                    }
                    else
                    {
                        _semaphore.Release();
                        _cancellationEvent.Wait(_millisecondsToSleep);
                    }
                }
                catch (Exception ex)
                {
                    OnExceptionOccurredInSchedulerThread(new ThrottledEventArgs<T>(ex));
                    Threading.ExecuteWithoutThrowing(() => _semaphore.Release());
                    _cancellationEvent.Wait(_millisecondsToSleep);      //It's good to sleep when an exception happens to avoid tight loops due to network faults.
                }
            }
        }

        /// <summary>
        /// This occurs in a background thread.  Can't let an exception out of this scope, it will crash the process.
        /// </summary>
        /// <param name="message"></param>
        private void ProcessAsync(T message, CancellableTask task)
        {
            try
            {
                OnMessageProcessing(new ThrottledEventArgs<T>(message));
                _processor(message);
                OnMessageProcessed(new ThrottledEventArgs<T>(message));
            }
            catch (Exception ex)
            {
                OnExceptionOccurredInWorkerThread(new ThrottledEventArgs<T>(message, ex));
                if (ex is ThreadAbortException)
                {
                    Thread.ResetAbort();    //I don't want an exception getting out of here.
                }
            }
            finally
            {
                Threading.ExecuteWithoutThrowing(() => _semaphore.Release());
                lock (_lockObject)
                {
                    _currentTasks.RemoveAll(x => x == task);
                }
            }
        }

        /// <summary>
        /// Synchronously stops the processor.  Blocking call, may take a long time.
        /// </summary>
        public void StopListening()
        {
            _cancellationTokenSource.Cancel();
            bool finished = false;
            do
            {
                lock (_lockObject)
                {
                    finished = (_currentTasks.Count == 0);
                }
                if (!finished)
                {
                    Thread.Sleep(50);       //This isn't ideal but we basically want to spin 
                }
            } while (!finished);
            _cancellationTokenSource.Dispose();
            //It's critical that the timer is stopped AFTER tasks are finished, because we may need the timer termination event to ensure the tasks stop.
            _taskKillingTimer.Stop(); 
            _taskKillingTimer.Dispose();
            _cancellationEvent.Dispose();
            _semaphore.Dispose();
            OnStoppedListeningEventFired(new ThrottledEventArgs<T>());
            _running = false;
        }
                
        /// <summary>
        /// Stops listening asynchronously.  Use only if you don't care about when it finishes or you've subscribed to the StoppedListeningEvent.
        /// </summary>
        public void StopListeningAsync()
        {
            Task.Run(StopListening);
        }

        /// <summary>
        /// Forces stop by multiple thread abort calls.  Not recommended, but will try to stop in a timely fashion.
        /// </summary>
        public void StopForcibly()
        {
            //Need to cancel before terminating the threads, otherwise it may start new tasks right after terminating old ones.
            _cancellationTokenSource.Cancel();
            lock (_lockObject)
            {
                foreach(var task in _currentTasks)
                {
                    task.Cancel();
                }
            }
            StopListening();
        }

    }
}
