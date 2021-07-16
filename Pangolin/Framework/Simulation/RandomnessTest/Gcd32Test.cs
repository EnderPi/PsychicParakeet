using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    [Serializable]
    [DataContract(Name = "GcdTest", Namespace = "EnderPi")]
    public class Gcd32Test : IIncremental32RandomTest
    {
        /// <summary>
        /// The expected frequencies of GCD.
        /// </summary>
        [DataMember]
        private double[] _expectedFrequencies;
                
        /// <summary>
        /// The cap on GCD values.
        /// </summary>
        [DataMember]
        private int _arraySize;
                
        /// <summary>
        /// Numbers to be processed
        /// </summary>
        [DataMember]
        private Queue<UInt32> _numbers;

        /// <summary>
        /// The count of GCD's
        /// </summary>
        [DataMember]
        private UInt64[] _gcds;
                
        /// <summary>
        /// Current number of GCDs calculated.  May be less than half the amount of numbers processed if given a lot of zeros.
        /// </summary>
        private UInt64 _iterationsPerformed;

        private ChiSquaredResult _chiSquaredGcd;
                
        /// <summary>
        /// The detailed result.  Might not be needed.
        /// TODO FRY THIS AND BUILD ON DEMAND
        /// </summary>
        private string _detailedResult;

        /// <summary>
        /// COnstructor for the test.  Only parameter right now is the cap on GCDs.  
        /// </summary>
        /// <param name="gcdCap">Limit on Gcds to track.  Default value is good for n=1 trillion</param>
        public Gcd32Test(int gcdCap = 348691)
        {
            _arraySize = gcdCap;
            _chiSquaredGcd = new ChiSquaredResult() { Result = TestResult.Inconclusive };            
        }

        /// <summary>
        /// Calculates and returns the result.
        /// </summary>
        /// <returns></returns>
        public void CalculateResult(bool detailed)
        {
            if (_iterationsPerformed < 100)
            {
                return;
            }

            _chiSquaredGcd = ChiSquaredTest.ChiSquaredPValueDetailed(_expectedFrequencies, _gcds, _iterationsPerformed, 5, detailed, 20);            
            _detailedResult = GetDetailedResult();
            return;
        }

        public TestResult Result { get { return _chiSquaredGcd.Result; } }

        public int TestsPassed => Result == TestResult.Fail ? 0 : 1;

        /// <summary>
        /// Gets the detailed result.
        /// </summary>
        /// <returns></returns>
        private string GetDetailedResult()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Gcd Test with N = {_iterationsPerformed} GCDs calculated");
            sb.AppendLine($"p-value for GCD {_chiSquaredGcd.PValue}");
            sb.AppendLine($"Result of {_chiSquaredGcd.Result} for GCD");
            foreach (var item in _chiSquaredGcd.TopContributors)
            {
                sb.AppendLine($"Contributor GCD {item.Index} with expected count {item.ExpectedCount} and actual count {item.ActualCount}, {Math.Round(item.FractionOfChiQuared * 100, 2)}% of ChiSquared");
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Initializes all the arrays.
        /// </summary>
        public void Initialize()
        {
            _gcds = new ulong[_arraySize];
            
            _expectedFrequencies = new double[_arraySize];
            _numbers = new Queue<uint>(4);

            CalculateExpectedFrequencies();
        }

        /// <summary>
        /// Populates the expected frequencies arrays.
        /// </summary>
        private void CalculateExpectedFrequencies()
        {
            double lastFrequency = 1.0;
            for (int i = 1; i < _expectedFrequencies.Length - 1; i++)
            {
                _expectedFrequencies[i] = 6.0 / System.Math.Pow(System.Math.PI * i, 2);
                lastFrequency -= _expectedFrequencies[i];
            }
            _expectedFrequencies[_arraySize - 1] = lastFrequency;            

        }

        /// <summary>
        /// Processes a random number, incrementing internal state accordingly.
        /// </summary>
        /// <param name="randomNumber"></param>
        public void Process(uint randomNumber)
        {
            if (randomNumber != 0)
            {
                _numbers.Enqueue(randomNumber);
                if (_numbers.Count >= 2)
                {
                    RunOneIteration();
                }
            }
        }

        /// <summary>
        /// Calculates one GCD and updates the internal state array.
        /// </summary>
        private void RunOneIteration()
        {
            UInt32 x = _numbers.Dequeue();
            UInt32 y = _numbers.Dequeue();
            int steps;
            UInt64 gcd = TestHelper.GreatestCommonDivisor(x, y, out steps);
            if (gcd > (UInt64)(_gcds.Length - 1))
            {
                gcd = (UInt64)_gcds.Length - 1;
            }
            _gcds[gcd]++;
            _iterationsPerformed++;
        }

        /// <summary>
        /// Stores the final results in the database.
        /// </summary>
        /// <param name="backgroundTaskId"></param>
        /// <param name="provider"></param>
        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {
            if (persistState)
            {
                GcdTestPoco gcdTestPoco = new GcdTestPoco() { SimulationId = backgroundTaskId, NumberOfGcds = Convert.ToInt64(_iterationsPerformed), PValueGcd = _chiSquaredGcd.PValue, PValueSTeps = 0, TestResultGcd = _chiSquaredGcd.Result, TestResultSteps = TestResult.Inconclusive, DetailedResult = _detailedResult };
                IGcdDataAccess gcdDataAccess = provider.GetService<IGcdDataAccess>();
                gcdDataAccess.CreateGcdTest(gcdTestPoco);
                foreach (var item in _chiSquaredGcd.TopContributors)
                {
                    GcdChiSquaredPoco poco = new GcdChiSquaredPoco() { Gcd = item.Index, Count = item.ActualCount, ExpectedCount = item.ExpectedCount, FractionOfChiSquared = item.FractionOfChiQuared, SimulationId = backgroundTaskId };
                    gcdDataAccess.CreateGcdChiSquared(poco);
                }
            }
            //write steps?
        }
    }
}
