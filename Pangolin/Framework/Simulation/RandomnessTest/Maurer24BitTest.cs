using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Services;
using MathNet.Numerics;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    [Serializable]
    public class Maurer24BitTest : IIncrementalRandomTest
    {
        private UInt64[] _indices;
        private int _wordSize;
        private UInt64 _iterationsPerformed;
        private UInt64 _minimumIterationsForResult;
        private double _pValue;
        private TestResult _testResult;
        private UInt64 _cellCount;
        private string _detailedResult;
        private bool _isInitialized;
        private ulong _requiredNumberToInitialize;
        private SumAccumulator _sumAccumulator;
        private double _mean;
        private double _sigma;
        private Queue<byte> _buffer;

        public double PValue
        {
            get
            {
                return _pValue;
            }
        }

        public string DetailedResult
        {
            get
            {
                return _detailedResult;
            }
        }

        public TestResult Result
        {
            get
            {
                return _testResult;
            }
        }

        public int TestsPassed => Result == TestResult.Pass ? 1 : 0;

        public Maurer24BitTest()
        {
            _wordSize = 24;
            _cellCount = Convert.ToUInt64(Math.Pow(2, _wordSize));
            _buffer = new Queue<byte>(16);
            CalculateMean();
            CalculateSigma();
        }

        private string GetDetailedResult()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Maurer Test with word size = {_wordSize}");
            sb.AppendLine($"p-value {_pValue}");
            sb.AppendLine($"Result of {_testResult}");
            return sb.ToString();
        }

        public void CalculateResult(bool detailed)
        {
            if (_iterationsPerformed < _minimumIterationsForResult)
            {
                return;
            }

            GetPValue();
            _testResult = TestHelper.GetTestResultFromPValue(_pValue);
            _detailedResult = GetDetailedResult();
        }


        private void GetPValue()
        {
            ulong n = _iterationsPerformed - _requiredNumberToInitialize;
            double fn = _sumAccumulator.Sum / n;
            _pValue = SpecialFunctions.Erfc(Math.Abs((fn - _mean) / (Math.Sqrt(2) * _sigma)));
            return;
        }

        private void CalculateSigma()
        {
            //sigma = c(l,k) * sqrt(var(an)/k)
            double d = 0.3920729;
            double e = 0.3592016;
            double c = Math.Sqrt(d + (e * _cellCount) / (_iterationsPerformed - _requiredNumberToInitialize));
            double varAn = 3.4237147;
            _sigma = c * Math.Sqrt(varAn / (_iterationsPerformed - _requiredNumberToInitialize));            
        }

        private void CalculateMean()
        {
            _mean = 24 - 0.8327462;
        }

        public void Process(UInt64 randomNumber)
        {
            var bytes = BitConverter.GetBytes(randomNumber);
            foreach (var b in bytes)
            {
                _buffer.Enqueue(b);
            }
            while (_buffer.Count >= 3)
            {
                int a = _buffer.Dequeue();
                int b = _buffer.Dequeue();
                int c = _buffer.Dequeue();
                int value = a << 16 | b << 8 | c;
                UInt64 difference = _iterationsPerformed - _indices[value];
                _indices[value] = _iterationsPerformed;
                if (_isInitialized)
                {
                    _sumAccumulator.Process(Math.Log(difference, 2));
                }
                _iterationsPerformed++;
                if (_iterationsPerformed > _requiredNumberToInitialize)
                {
                    _isInitialized = true;
                }
            }
        }


        public override string ToString()
        {
            return $"Maurer 16 bit Test Incremental - Word Size {_wordSize}";
        }

        public void Initialize()
        {
            _indices = new UInt64[_cellCount];
            _iterationsPerformed = 0;
            _minimumIterationsForResult = 1010 * _cellCount;
            _requiredNumberToInitialize = 10 * _cellCount;
            _sumAccumulator = new SumAccumulator();

            _testResult = TestResult.Inconclusive;
            _pValue = 0;
            _detailedResult = "";
            _isInitialized = false;
        }


        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {

        }
    }
}
