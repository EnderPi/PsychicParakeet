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
    public class GcdTest : IIncrementalRandomTest
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
        /// The cap on number of steps.
        /// </summary>
        private const int _kstepsarraySize = 70;

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
        public GcdTest(int gcdCap = 348691)
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
            foreach(var item in _chiSquaredGcd.TopContributors)
            {
                sb.AppendLine($"Contributor GCD {item.Index} with expected count {item.ExpectedCount} and actual count {item.ActualCount}, {Math.Round(item.FractionOfChiQuared*100,2)}% of ChiSquared" );
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
            _gcds = new ulong[_arraySize];
            _steps = new ulong[_kstepsarraySize];

            _expectedFrequencies = new double[_arraySize];
            _expectedStepFrequencies = new double[_kstepsarraySize];
            _numbers = new Queue<ulong>(4);
            
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

            //fill in expected k frequencies, empirically obtained from SHA256
            _expectedStepFrequencies[0] = 0;
            _expectedStepFrequencies[1] = 0;
            _expectedStepFrequencies[2] = 0;
            _expectedStepFrequencies[3] = 0;
            _expectedStepFrequencies[4] = 0;
            _expectedStepFrequencies[5] = 0;
            _expectedStepFrequencies[6] = 1.0000E-12;
            _expectedStepFrequencies[7] = 9.0000E-12;
            _expectedStepFrequencies[8] = 6.1000E-11;
            _expectedStepFrequencies[9] = 3.1300E-10;
            _expectedStepFrequencies[10] = 1.4520E-09;
            _expectedStepFrequencies[11] = 6.2210E-09;
            _expectedStepFrequencies[12] = 2.4481E-08;
            _expectedStepFrequencies[13] = 9.1230E-08;
            _expectedStepFrequencies[14] = 3.1099E-07;
            _expectedStepFrequencies[15] = 9.8756E-07;
            _expectedStepFrequencies[16] = 2.9229E-06;
            _expectedStepFrequencies[17] = 8.1186E-06;
            _expectedStepFrequencies[18] = 2.1158E-05;
            _expectedStepFrequencies[19] = 5.1885E-05;
            _expectedStepFrequencies[20] = 1.2003E-04;
            _expectedStepFrequencies[21] = 2.6239E-04;
            _expectedStepFrequencies[22] = 5.4279E-04;
            _expectedStepFrequencies[23] = 1.0644E-03;
            _expectedStepFrequencies[24] = 1.9814E-03;
            _expectedStepFrequencies[25] = 3.5051E-03;
            _expectedStepFrequencies[26] = 5.8989E-03;
            _expectedStepFrequencies[27] = 9.4532E-03;
            _expectedStepFrequencies[28] = 1.4438E-02;
            _expectedStepFrequencies[29] = 2.1034E-02;
            _expectedStepFrequencies[30] = 2.9248E-02;
            _expectedStepFrequencies[31] = 3.8841E-02;
            _expectedStepFrequencies[32] = 4.9290E-02;
            _expectedStepFrequencies[33] = 5.9801E-02;
            _expectedStepFrequencies[34] = 6.9394E-02;
            _expectedStepFrequencies[35] = 7.7048E-02;
            _expectedStepFrequencies[36] = 8.1876E-02;
            _expectedStepFrequencies[37] = 8.3299E-02;
            _expectedStepFrequencies[38] = 8.1151E-02;
            _expectedStepFrequencies[39] = 7.5718E-02;
            _expectedStepFrequencies[40] = 6.7673E-02;
            _expectedStepFrequencies[41] = 5.7941E-02;
            _expectedStepFrequencies[42] = 4.7525E-02;
            _expectedStepFrequencies[43] = 3.7345E-02;
            _expectedStepFrequencies[44] = 2.8112E-02;
            _expectedStepFrequencies[45] = 2.0272E-02;
            _expectedStepFrequencies[46] = 1.4002E-02;
            _expectedStepFrequencies[47] = 9.2615E-03;
            _expectedStepFrequencies[48] = 5.8657E-03;
            _expectedStepFrequencies[49] = 3.5561E-03;
            _expectedStepFrequencies[50] = 2.0633E-03;
            _expectedStepFrequencies[51] = 1.1453E-03;
            _expectedStepFrequencies[52] = 6.0799E-04;
            _expectedStepFrequencies[53] = 3.0854E-04;
            _expectedStepFrequencies[54] = 1.4960E-04;
            _expectedStepFrequencies[55] = 6.9289E-05;
            _expectedStepFrequencies[56] = 3.0629E-05;
            _expectedStepFrequencies[57] = 1.2916E-05;
            _expectedStepFrequencies[58] = 5.1925E-06;
            _expectedStepFrequencies[59] = 1.9879E-06;
            _expectedStepFrequencies[60] = 7.2459E-07;
            _expectedStepFrequencies[61] = 2.5073E-07;
            _expectedStepFrequencies[62] = 8.2720E-08;
            _expectedStepFrequencies[63] = 2.5610E-08;
            _expectedStepFrequencies[64] = 7.5580E-09;
            _expectedStepFrequencies[65] = 2.1830E-09;
            _expectedStepFrequencies[66] = 6.0200E-10;
            _expectedStepFrequencies[67] = 1.3500E-10;
            _expectedStepFrequencies[68] = 3.0000E-11;
            _expectedStepFrequencies[69] = 6.0000E-12;

        }

        /// <summary>
        /// Processes a random number, incrementing internal state accordingly.
        /// </summary>
        /// <param name="randomNumber"></param>
        public void Process(ulong randomNumber)
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
                GcdTestPoco gcdTestPoco = new GcdTestPoco() { SimulationId = backgroundTaskId, NumberOfGcds = Convert.ToInt64(_iterationsPerformed), PValueGcd = _chiSquaredGcd.PValue, PValueSTeps = _chiSquaredSteps.PValue, TestResultGcd = _chiSquaredGcd.Result, TestResultSteps = _chiSquaredSteps.Result, DetailedResult = _detailedResult };
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
