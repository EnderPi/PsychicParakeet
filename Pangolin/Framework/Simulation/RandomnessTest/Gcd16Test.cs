using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class Gcd16Test : IIncremental16RandomTest
    {
        /// <summary>
        /// The expected frequencies of GCD.
        /// </summary>
        [DataMember]
        private double[] _expectedFrequencies;

        /// <summary>
        /// The expected step frequencies.
        /// </summary>
        [DataMember]
        private double[] _expectedStepFrequencies;

        /// <summary>
        /// The cap on GCD values.
        /// </summary>
        [DataMember]
        private int _arraySize;
                

        /// <summary>
        /// Numbers to be processed
        /// </summary>
        [DataMember]
        private Queue<UInt64> _numbers;

        /// <summary>
        /// The count of GCD's
        /// </summary>
        [DataMember]
        private UInt64[] _gcds;

        /// <summary>
        /// The count of K Values
        /// </summary>
        [DataMember]
        private UInt64[] _steps;

        /// <summary>
        /// Current number of GCDs calculated.  May be less than half the amount of numbers processed if given a lot of zeros.
        /// </summary>
        private UInt64 _iterationsPerformed;

        private ChiSquaredResult _chiSquaredGcd;

        private ChiSquaredResult _chiSquaredSteps;


        /// <summary>
        /// The detailed result.  Might not be needed.
        /// TODO FRY THIS AND BUILD ON DEMAND
        /// </summary>
        private string _detailedResult;

        /// <summary>
        /// COnstructor for the test.  Only parameter right now is the cap on GCDs.  
        /// </summary>
        /// <param name="gcdCap">Limit on Gcds to track.  Default value is good for n=1 trillion</param>
        public Gcd16Test(int gcdCap = 348691)
        {
            _arraySize = gcdCap;
            _chiSquaredGcd = new ChiSquaredResult() { Result = TestResult.Inconclusive };
            _chiSquaredSteps = new ChiSquaredResult() { Result = TestResult.Inconclusive };
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
            _chiSquaredSteps = ChiSquaredTest.ChiSquaredPValueDetailed(_expectedStepFrequencies, _steps, _iterationsPerformed, 5, detailed, 20);
            _detailedResult = GetDetailedResult();
            return;
        }

        public TestResult Result { get { return TestHelper.ReturnLowestConclusiveResult(_chiSquaredGcd.Result, _chiSquaredSteps.Result); } }

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
            sb.AppendLine($"p-value for steps {_chiSquaredSteps.PValue}");
            sb.AppendLine($"Result of {_chiSquaredSteps.Result} for steps");
            foreach (var item in _chiSquaredGcd.TopContributors)
            {
                sb.AppendLine($"Contributor GCD {item.Index} with expected count {item.ExpectedCount} and actual count {item.ActualCount}, {Math.Round(item.FractionOfChiQuared * 100, 2)}% of ChiSquared");
            }
            foreach (var item in _chiSquaredSteps.TopContributors)
            {
                sb.AppendLine($"Contributor Steps {item.Index} with expected count {item.ExpectedCount} and actual count {item.ActualCount}, {Math.Round(item.FractionOfChiQuared * 100, 2)}% of ChiSquared");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Initializes all the arrays.
        /// </summary>
        public void Initialize()
        {
            _gcds = new ulong[65536];
            _steps = new ulong[25];

            _expectedFrequencies = Gcd16Literals.GetFrequencies();
            _expectedStepFrequencies = Gcd16Literals.GetStepFrequencies();
            _numbers = new Queue<ulong>(4);                        
        }                

        /// <summary>
        /// Processes a random number, incrementing internal state accordingly.
        /// </summary>
        /// <param name="randomNumber"></param>
        public void Process(ushort randomNumber)
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
            UInt64 x = _numbers.Dequeue();
            UInt64 y = _numbers.Dequeue();
            int steps;
            UInt64 gcd = TestHelper.GreatestCommonDivisor(x, y, out steps);
            if (gcd > (UInt64)(_gcds.Length - 1))
            {
                gcd = (UInt64)_gcds.Length - 1;
            }
            if (steps > _steps.Length - 1)
            {
                steps = _steps.Length - 1;
            }
            _gcds[gcd]++;
            _steps[steps]++;
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
            }
            //write steps?
        }
    }
}
