using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using System;
using System.Runtime.Serialization;
using System.Threading;

namespace EnderPi.Framework.Simulation
{
    /// <summary>
    /// Simulation that gets some birthday counts for a given N, and does it K times, then stores the results.  Used to develop values for a randomness test.
    /// </summary>
    [Serializable]
    [DataContract(Name = "GetBirthdayValuesSimulation", Namespace = "EnderPi")]
    public class GetBirthdayValuesSimulation : LongRunningTask
    {
        [DataMember]
        private ulong[] _birthdays;
        [DataMember]
        private ulong[] _spacings;
        [DataMember]
        private int _numberOfBirthdays;
        [DataMember]
        private int _numberOfIterations;
        [DataMember]
        private int _currentIterationCount;
        [DataMember]
        private IEngine _engine;
        [DataMember]
        private ulong _seed;


        public GetBirthdayValuesSimulation(ulong seed, IEngine engine, int numberOfIterations, int numberOfBirthdays)
        {
            _seed = seed;
            _engine = engine;
            _numberOfIterations = numberOfIterations;
            _numberOfBirthdays = numberOfBirthdays;
        }

        /// <summary>
        /// The initialize method should initialize any large state needed.  Won't be called if loading from disk.  Probably needs to be virtual from an abstract
        /// </summary>
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            _birthdays = new ulong[_numberOfBirthdays];
            _spacings = new ulong[_numberOfBirthdays - 1];
            _engine.Seed(_seed);
            if (persistState)
            {
                var birthdayDataAccess = provider.GetService<IEmpiricalBirthdayDataAccess>();
                birthdayDataAccess.WriteBirthdaySimulationRecord(backgroundTaskId, _seed, _numberOfBirthdays);
            }
        }

        /// <summary>
        /// So this should be a blocking call, and should be safe to call for the first time or if resuming.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="provider"></param>
        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {            
            //so for this particular simulation, individual iterations are completely separate, so it can actually store results in the database as it goes.
            //cancellation, for this task, will preserve state
            var backGroundTaskManager = provider.GetService<BackgroundTaskManager>();
            while (_currentIterationCount < _numberOfIterations  && !token.IsCancellationRequested)
            {
                int duplicates = DoOneIteration();
                _currentIterationCount++;                
                if (persistState)
                {
                    StoreResults(duplicates, provider, backgroundTaskId);
                    backGroundTaskManager.SaveIfNecessary(this, backgroundTaskId);
                    backGroundTaskManager.ReportProgress(backgroundTaskId, 100 * (double)_currentIterationCount / _numberOfIterations);
                }
            }
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //Nothing to do here, results are stored as they come in for this task.
        }

        private int DoOneIteration()
        {
            for (int i=0; i <_numberOfBirthdays; i++)
            {
                _birthdays[i] = _engine.Next64();
            }
            Array.Sort(_birthdays);
            for (int i=0; i<_numberOfBirthdays - 1; i++)
            {
                _spacings[i] = _birthdays[i + 1] - _birthdays[i];
            }
            Array.Sort(_spacings);
                        
            return Collections.Utility.CountNumberOfDuplicatedElements(_spacings);
        }

        private void StoreResults(int duplicates, ServiceProvider provider, int backgroundTaskId)
        {
            var birthdayDataAccess = provider.GetService<IEmpiricalBirthdayDataAccess>();
            birthdayDataAccess.WriteDuplicatesForBirthdaySimulation(backgroundTaskId, _currentIterationCount, duplicates);
        }
                



    }
}
