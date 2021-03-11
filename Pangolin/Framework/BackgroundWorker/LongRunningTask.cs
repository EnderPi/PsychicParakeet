using System;
using System.Threading;
using EnderPi.Framework.Services;
using System.Runtime.Serialization;
using EnderPi.Framework.Simulation;

namespace EnderPi.Framework.BackgroundWorker
{
    /// <summary>
    /// Parent class for long running tasks?  I think we need this so that the backgroundtaskruntime can make assumptions and call methods.
    /// </summary>
    [Serializable]
    [DataContract(Name = "LongRunningTask", Namespace = "EnderPi")]
    [KnownType(typeof(GetBirthdayValuesSimulation))]
    [KnownType(typeof(RandomnessSimulation))]
    [KnownType(typeof(TestSimulation))]
    public abstract class LongRunningTask
    {
        /// <summary>
        /// Subclasses override this to initialize any large state.
        /// </summary>
        protected abstract void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState);

        /// <summary>
        /// Whether or not this task is initialized.
        /// </summary>
        [DataMember]
        private bool _isInitialized;

        /// <summary>
        /// Overall method for starting the task.  Contains the general workflow.
        /// </summary>
        /// <param name="token">A cancellation token for cooperative cancellation.</param>
        /// <param name="provider">A service provider for any needed dependencies.</param>
        /// <param name="backgroundTaskId">The background task ID associated with this task.</param>
        /// <param name="persistState">Pass true if this task should persist state to the database, false otherwise.</param>
        public void Start(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            if (!_isInitialized)
            {
                InitializeInternal(token, provider, backgroundTaskId, persistState);
                _isInitialized = true;
            }
            StartInternal(token, provider, backgroundTaskId, persistState);
            if (!token.IsCancellationRequested)
            {
                StoreFinalResults(provider, backgroundTaskId, persistState);
            }
        }

        /// <summary>
        /// Subclasses override this to implement their own task logic.
        /// </summary>
        /// <param name="token">A cancellation token for cooperative cancellation.</param>
        /// <param name="provider">A service provider for any needed dependencies.</param>
        /// <param name="backgroundTaskId">The backgroudn task ID associated with this task.</param>
        protected abstract void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState);

        /// <summary>
        /// Subclasses should override this to store final results in the database.
        /// </summary>
        protected abstract void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState);

    }
}
