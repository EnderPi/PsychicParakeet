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
    /// <summary>
    /// The birthday test as described by Marsaglia.
    /// </summary>
    [Serializable]
    [DataContract(Name = "BirthdayTest", Namespace = "EnderPi")]
    public class BirthdayTest : IIncrementalRandomTest
    {

        private int M = 8388608;
        private UInt64 N = UInt64.MaxValue;
        private List<UInt64> _Birthdays;
        private ulong[] _poissonValues;
        private double[] _frequencies;
        private const int _lengthOfFrequencyArray = 22;     //22 ensures that the top bin has 5 expected values for 1 trillion deviates
        private double _pValue;
        private TestResult _testResult;
        private string _detailedResult;
        private int _currentIterations;
        private int _lambda = 8;

        public BirthdayTest()
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
                for (int i=0; i<_poissonValues.Length; i++)
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
            _frequencies = new double[_lengthOfFrequencyArray];
            double lambda = M * (double)M * M / (4.0 * N);
            double remainder = 1;
            for (int i = 0; i < _frequencies.Length - 1; i++)
            {
                _frequencies[i] = Math.Exp(-lambda) * Math.Pow(lambda, i) / SpecialFunctions.Factorial(i);
                remainder -= _frequencies[i];
            }
            _frequencies[_lengthOfFrequencyArray - 1] = remainder;
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
                
        public void Process(ulong randomNumber)
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
