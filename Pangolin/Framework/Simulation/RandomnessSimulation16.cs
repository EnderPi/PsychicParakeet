using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace EnderPi.Framework.Simulation
{
    public class RandomnessSimulation16 : LongRunningTask
    {
        [DataMember]
        private List<IIncremental16RandomTest> _tests;
        [DataMember]
        private long _targetNumberOfIterations;
        [DataMember]
        private long _currentNumberOfIterations;
        [DataMember]
        private Engine16 _randomEngine;

        //TODO feels like this needs to be an actual object that decides whether or not to stop, or maybe an enum or something, like fixed or multiplicative?
        //but those are just object sub-types.  should be an interface.
        [DataMember]
        private long _nextIterationCheck;
        [DataMember]
        private TestResult _overallResult;
        [DataMember]
        private ulong _seed;
        [DataMember]
        private string _description;
        [DataMember]
        private TestLevel _testLevel;


        public TestResult Result { get { return _overallResult; } }

        public long Iterations { get { return _currentNumberOfIterations; } }

        public int TestsPassed
        {
            get
            {
                return _tests.Sum(x => x.TestsPassed);
            }

        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="tests"></param>
        /// <param name="targetNumberOfIterations"></param>
        /// <param name="engine"></param>
        /// <param name="iterationsBetweenChecks"></param>
        /// <param name="seed"></param>
        /// <param name="description"></param>
        public RandomnessSimulation16(List<IIncremental16RandomTest> tests, long targetNumberOfIterations, Engine16 engine, ulong seed, string description)
        {
            _tests = tests;
            _targetNumberOfIterations = targetNumberOfIterations;
            _randomEngine = engine;
            _seed = seed;
            _description = description;
            _testLevel = TestLevel.Adhoc;
        }

        public RandomnessSimulation16(TestLevel level, Engine16 engine, ulong seed)
        {
            _randomEngine = engine;
            _seed = seed;
            _tests = new List<IIncremental16RandomTest>();
            _testLevel = level;
            switch (level)
            {
                case TestLevel.Adhoc:
                    throw new ArgumentOutOfRangeException(nameof(level));
                case TestLevel.One:
                    _tests.Add(new Gcd16Test());                    
                    _targetNumberOfIterations = Convert.ToInt64(1e3);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Two:
                    _tests.Add(new Gcd16Test());
                    _tests.Add(new GorillaTest16(8));
                    _tests.Add(new GorillaTest16(16));
                    _tests.Add(new BirthdayTest16());
                    _tests.Add(new MaurerTest16());
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Gcd:
                    _tests.Add(new Gcd16Test());
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Gorilla8:
                    _tests.Add(new GorillaTest16(8));
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Gorilla16:
                    _tests.Add(new GorillaTest16(16));
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Birthday:
                    _tests.Add(new BirthdayTest16());
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Maurer16:
                    _tests.Add(new MaurerTest16());
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
                case TestLevel.Maurer8:
                    _tests.Add(new MaurerTest16_8Bit());
                    _targetNumberOfIterations = Convert.ToInt64(1e8);
                    _description = $"Level 1 Test, Engine: {_randomEngine}, Seed: {_seed}";
                    break;
            }
        }

        /// <summary>
        /// Mostly initializes sub-tests, seeds the generator.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="backgroundTaskId"></param>
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            _randomEngine.Seed(_seed);
            _overallResult = TestResult.Inconclusive;
            _nextIterationCheck = 50;
            foreach (var test in _tests)
            {
                test.Initialize();
            }
        }

        /// <summary>
        /// Starts the simulation.  Blocking call.  Always stops on failure.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="provider"></param>
        /// <param name="backgroundTaskId"></param>
        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            var backgroundTaskManager = provider.GetService<IBackgroundTaskManager>();
            while (!token.IsCancellationRequested && _currentNumberOfIterations < _targetNumberOfIterations && _overallResult != TestResult.Fail)
            {
                DoOneIteration();
                if (_currentNumberOfIterations == _nextIterationCheck)
                {
                    _nextIterationCheck += _nextIterationCheck / 10;
                    CalculateResults(false);
                    if (persistState)
                    {
                        backgroundTaskManager.ReportProgress(backgroundTaskId, 100 * (double)_currentNumberOfIterations / _targetNumberOfIterations);
                        backgroundTaskManager.SaveIfNecessary(this, backgroundTaskId);
                    }
                }
            }
        }

        /// <summary>
        /// Does one iteration of the simulation, generating one random number and pushing it to each test.
        /// </summary>
        private void DoOneIteration()
        {
            var randomNumber = _randomEngine.Next16();
            foreach (var test in _tests)
            {
                test.Process(randomNumber);
            }
            _currentNumberOfIterations++;
        }


        private void CalculateResults(bool finalize)
        {
            List<TestResult> testResults = new List<TestResult>();
            foreach (var test in _tests)
            {
                test.CalculateResult(finalize);
                testResults.Add(test.Result);
            }
            _overallResult = TestHelper.ReturnLowestConclusiveResultEnumerable(testResults);
        }

        public string GetFailedTest()
        {
            throw new NotImplementedException();
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            CalculateResults(true);
            if (persistState)
            {
                RandomnessSimulationPoco poco = new RandomnessSimulationPoco() { Description = _description, NumbersGenerated = _currentNumberOfIterations, TargetNumbersGenerated = _targetNumberOfIterations, Result = _overallResult, RandomNumberEngine = _randomEngine.ToString() };
                IRandomnessSimulationDataAccess dataAccess = provider.GetService<IRandomnessSimulationDataAccess>();
                dataAccess.CreateRandomnessSimulation(poco);
            }
            foreach (var test in _tests)
            {
                //todo pass persist state into each of these.
                test.CalculateResult(true);
                test.StoreFinalResults(backgroundTaskId, provider, persistState);
            }

        }
    }
}
