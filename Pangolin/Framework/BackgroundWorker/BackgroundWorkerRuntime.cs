using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Messaging.Events;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Services;
using EnderPi.Framework.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EnderPi.Framework.Messaging;

namespace EnderPi.Framework.BackgroundWorker
{
    /// <summary>
    /// The runtime for the background worker.  Stores all the state, mostly wraps a throttled task processor with the right behavior.
    /// Manages *very* long running tasks, things that may need to be restarted from a file, etc.  Manages individual cancellation as well.
    /// </summary>
    public class BackgroundWorkerRuntime
    {

        private List<RunningSimulationReference> _runningSimulations;

        private object _simulationPadLock;

        private ThrottledTaskProcessor<RunningSimulationReference> _throttledTaskProcessor;

        private BackgroundTaskManager _taskManager;

        private ServiceProvider _serviceProvider;

        private Logger _logger;

        private IConfigurationDataAccess _configurationDataAccess;

        private EventManager _eventManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundWorkerRuntime(BackgroundTaskManager taskManager, ThrottledTaskProcessorParameters taskParameters, ServiceProvider serviceProvider, Logger logger, IConfigurationDataAccess configurationDataAccess, EventManager eventManager)
        {
            _runningSimulations = new List<RunningSimulationReference>();
            _simulationPadLock = new object();
            _taskManager = taskManager;
            _throttledTaskProcessor = new ThrottledTaskProcessor<RunningSimulationReference>(GetNextTask, ProcessBackgroundTask, taskParameters);
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configurationDataAccess = configurationDataAccess;
            //event hooks for processor.
            _throttledTaskProcessor.ExceptionOccurredInWorkerThread += LogException;
            _throttledTaskProcessor.ExceptionOccurredInWorkerThread += HandleErrorFromWorkerThread;
            _throttledTaskProcessor.ExceptionOccurredInSchedulerThread += LogException;
            _throttledTaskProcessor.MessageProcessed += TaskFinishedSuccessfully;
            _throttledTaskProcessor.TaskFinished += TaskCompleted;
            //TODO fix the fact that the throttled task processor should have a logger and be doing all the logging, maybe annotating with type or name or something?
            eventManager.Subscribe<SimulationCancelledEvent>(CancelSimulation);
            eventManager.Subscribe<StopBackgroundTasksEvent>(StopAllTasks);
            _eventManager = eventManager;
        }

        /// <summary>
        /// Cancels all events.  Attempts to stop gracefully.
        /// </summary>
        /// <param name="obj"></param>
        private void StopAllTasks(StopBackgroundTasksEvent obj)
        {
            lock(_simulationPadLock)
            {
                foreach (var simulationReference in _runningSimulations)
                {
                    simulationReference.CancellationSource.Cancel();
                }
            }            
        }

        private void TaskFinishedSuccessfully(object sender, ThrottledEventArgs<RunningSimulationReference> e)
        {
            if (e.Message != null)
            {
                var simulation = _taskManager.GetTask(e.Message.SimulationId);
                DeleteSaveFile(simulation);
                _taskManager.MarkSimulationComplete(e.Message.SimulationId);
                SimulationFinishedEvent simEvent = new SimulationFinishedEvent() { SimulationId = e.Message.SimulationId };
                _eventManager.PublishEvent(simEvent);
            }
        }

        /// <summary>
        /// Deletes the save file for the simulation.
        /// </summary>
        /// <param name="simulation"></param>
        private void DeleteSaveFile(SimulationPoco simulation)
        {
            if (simulation != null)
            {
                try
                {
                    if (File.Exists(simulation.SaveFile))
                    {
                        File.Delete(simulation.SaveFile);
                    }
                }
                catch (Exception ex)
                {
                    LogDetails details = new LogDetails();
                    details.AddDetail("Exception", ex.ToString());
                    details.AddDetail("Id", simulation.Id.ToString());
                    details.AddDetail("Filename", simulation.SaveFile);
                    _logger.Log($"Failed to delete save file {simulation.SaveFile} for {simulation.Id}", LoggingLevel.Warning);
                }
            }
        }

        /// <summary>
        /// This event runs whether or not the task completed successfully, so it shouldn't do much other than remove it from the in-memory collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskCompleted(object sender, ThrottledEventArgs<RunningSimulationReference> e)
        {
            if (e.Message != null)
            {
                if (e.Message.CancellationSource.IsCancellationRequested)
                {
                    SaveTask(e.Message);
                }
                lock (_simulationPadLock)
                {
                    _runningSimulations.RemoveAll(x => x.SimulationId == e.Message.SimulationId);
                }
                e.Message.CancellationSource.Dispose();
            }            
        }

        /// <summary>
        /// Saves the task to a file, and writes the filename to the daterbase.
        /// </summary>
        /// <param name="message"></param>
        private void SaveTask(RunningSimulationReference message)
        {
            _taskManager.SaveTask(message.Task, message.SimulationId);            
        }


        /// <summary>
        /// Cancels the requested task by calling cancel on the token.  This task will not restart unless re-submitted.
        /// </summary>
        /// <param name="e"></param>
        private void CancelSimulation(SimulationCancelledEvent e)
        {
            _taskManager.MarkSimulationAsCanceled(e.SimulationId);      //mark it as canceled prior to messing with the async stuff
            lock (_simulationPadLock)
            {
                var task = _runningSimulations.FirstOrDefault(x => x.SimulationId == e.SimulationId);
                if (task != null)
                {
                    task.CancellationSource.Cancel();
                }
                else
                {
                    //This is only a warning since it may have finished prior to being cancelled due to a race condition.
                    _logger.Log($"Failed to find simulation ID {e.SimulationId} to cancel.", LoggingLevel.Warning);
                }
            }            
        }

        /// <summary>
        /// Logs exceptions coming from the underlying throttled event processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogException(object sender, ThrottledEventArgs<RunningSimulationReference> e)
        {
            if (e != null && e.Exception != null)
            {
                _logger.Log(e.Exception.ToString(), LoggingLevel.Error);
            }
        }

        /// <summary>
        /// Logs exceptions coming from the underlying throttled event processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleErrorFromWorkerThread(object sender, ThrottledEventArgs<RunningSimulationReference> e)
        {
            if (e != null && e.Message != null)
            {
                _taskManager.MarkSimulationAsCanceled(e.Message.SimulationId);
                _logger.Log($"Exception from simulation, marking as canceled - Simulation ID {e.Message.SimulationId}", LoggingLevel.Error);
            }
        }

        private RunningSimulationReference GetNextTask()
        {
            RunningSimulationReference task = null;
            lock (_simulationPadLock)
            {
                task = _taskManager.GetNextTask(_runningSimulations);
                if (task != null)
                {
                    task.CancellationSource = new CancellationTokenSource();
                    _runningSimulations.Add(task);
                }
            }
            return task;
        }

        private void ProcessBackgroundTask(RunningSimulationReference task)
        {
            task.Task.Start(task.CancellationSource.Token, _serviceProvider, task.SimulationId, true);
        }


        public void Start()
        {
            _throttledTaskProcessor.StartListening();
        }


        public void Stop()
        {
            _throttledTaskProcessor.StopListening();
        }       

    }
}
