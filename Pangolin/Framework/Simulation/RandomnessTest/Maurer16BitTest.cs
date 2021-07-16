﻿using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Services;
using MathNet.Numerics;


namespace EnderPi.Framework.Simulation.RandomnessTest
{
    [Serializable]
    public class Maurer16BitTest : IIncrementalRandomTest
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

        public Maurer16BitTest()
        {
            _wordSize = 16;
            _cellCount = Convert.ToUInt64(Math.Pow(2, _wordSize));            
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
            double sigma = CalculateSigma();
            double mean = CalculateMean();
            ulong n = _iterationsPerformed - _requiredNumberToInitialize;
            double fn = _sumAccumulator.Sum / n;
            _pValue = SpecialFunctions.Erfc(Math.Abs((fn - mean) / (Math.Sqrt(2) * sigma)));
            return;
        }

        private double CalculateSigma()
        {
            //sigma = c(l,k) * sqrt(var(an)/k)
            double d = _wordSize == 8 ? 0.3732189 : 0.3917561;
            double e = _wordSize == 8 ? 0.3730195 : 0.3594040;
            double c = Math.Sqrt(d + (e * _cellCount) / (_iterationsPerformed - _requiredNumberToInitialize));
            double varAn = _wordSize == 8 ? 3.2386622 : 3.4213083;
            double sigma = c * Math.Sqrt(varAn / (_iterationsPerformed - _requiredNumberToInitialize));
            return sigma;
        }

        private double CalculateMean()
        {
            var checker = new ConvergenceChecker(4, .00000000000001, ConvergenceType.Relative);
            var sumAccumulator = new SumAccumulator();
            double constant = Math.Pow(2, -_wordSize);
            int i;
            for (i = 1; !checker.IsConverged(sumAccumulator.Sum); i++)
            {
                sumAccumulator.Process(Math.Pow(1 - constant, i - 1) * Math.Log(i, 2));
            }
            var value = sumAccumulator.Sum * constant;
            return value;
        }

        public void Process(UInt64 randomNumber)
        {
            var bytes = BitConverter.GetBytes(randomNumber);
            int[] words = new int[4];
            words[0] = BitConverter.ToUInt16(bytes, 0);
            words[1] = BitConverter.ToUInt16(bytes, 2);
            words[2] = BitConverter.ToUInt16(bytes, 4);
            words[3] = BitConverter.ToUInt16(bytes, 6);
            foreach (var value in words)
            {
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
