using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using MathNet.Numerics;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class BirthdayTest16 :IIncremental16RandomTest
    {
        private int M = 128;
        private UInt64 N = ushort.MaxValue;
        private List<UInt64> _Birthdays;
        private ulong[] _poissonValues;
        private double[] _frequencies;
        private const int _lengthOfFrequencyArray = 22;     //arbitrary
        private double _pValue;
        private TestResult _testResult;
        private string _detailedResult;
        private int _currentIterations;
        private int _lambda = 1;

        public BirthdayTest16()
        {
        }

        public TestResult Result { get { return _testResult; } }

        public int TestsPassed => _testResult == TestResult.Fail ? 0 : 1;

        public void CalculateResult(bool detailed)
        {
            if (_currentIterations < 50)
            {
                return;
            }

            _pValue = TestHelper.ChiSquaredPValue(_frequencies, _poissonValues, Convert.ToUInt64(_currentIterations));
            _testResult = TestHelper.GetTestResultFromPValue(_pValue);
            _detailedResult = GetDetailedResult();
            return;
        }

        public void Initialize()
        {
            CalculateFrequencyArray();
            _Birthdays = new List<ulong>(M);
            _poissonValues = new ulong[_lengthOfFrequencyArray];
            _currentIterations = 0;
            _testResult = TestResult.Inconclusive;
            _detailedResult = "";
            _pValue = 0;
        }

        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {
            if (persistState)
            {
                var birthdayDataAccess = provider.GetService<IBirthdayDataAccess>();
                BirthdayTestPoco testPoco = new BirthdayTestPoco() { SimulationId = backgroundTaskId, NumberOfIterations = _currentIterations, Lambda = _lambda, PValue = _pValue, Result = _testResult, DetailedResult = _detailedResult };
                birthdayDataAccess.CreateBirthdayTest(testPoco);
                for (int i = 0; i < _poissonValues.Length; i++)
                {
                    var detailPoco = new BirthdayTestDetailPoco() { SimulationId = backgroundTaskId, NumberOfDuplicates = i, ExpectedNumberOfDuplicates = _frequencies[i] * _currentIterations, ActualNumberOfDuplicates = Convert.ToInt32(_poissonValues[i]) };
                    birthdayDataAccess.CreateBirthdayTestDetail(detailPoco);
                }
            }
        }

        /// <summary>
        /// Populates the frequency array - the theoretical frequencies for the given lambda.
        /// </summary>
        private void CalculateFrequencyArray()
        {
            _frequencies = new double[] {0.0002861,
0.0027424
,0.0126953
,0.037463
,0.0793828
,0.1283207
,0.1644148
,0.1725313
,0.1508062
,0.111578
,0.0707673
,0.0387861
,0.0184622
,0.0076501
,0.002833
,0.0009284
,0.000262
,6.98E-05
,1.67E-05
,3.2E-06
,4E-07
,2E-07 };
            
        }

        private string GetDetailedResult()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Birthday Test with M = {M}, Lambda = {_lambda}");
            sb.AppendLine($"{_currentIterations} iterations");
            sb.AppendLine($"p-value {_pValue}");
            sb.AppendLine($"Result of {_testResult}");
            return sb.ToString();
        }

        public void Process(ushort randomNumber)
        {
            _Birthdays.Add(randomNumber);
            if (_Birthdays.Count == M)
            {
                RunOneIteration();
            }
        }

        private void RunOneIteration()
        {
            _Birthdays.Sort();
            ulong[] spacings = new ulong[M - 1];
            for (int j = 0; j < M - 1; j++)
            {
                spacings[j] = _Birthdays[j + 1] - _Birthdays[j];
            }
            Array.Sort(spacings);
            int duplicates = Collections.Utility.CountNumberOfDuplicatedElements(spacings);
            if (duplicates > _lengthOfFrequencyArray - 1)
            {
                duplicates = _lengthOfFrequencyArray - 1;
            }
            _poissonValues[duplicates]++;
            _currentIterations++;
            _Birthdays.Clear();
        }

        public void Reset()
        {

        }
    }
}
