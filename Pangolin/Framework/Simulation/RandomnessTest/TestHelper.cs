using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    /// <summary>
    /// A collection of helper functions for test use.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Returns the worst conclusive test result.
        /// </summary>
        /// <param name="testResults"></param>
        /// <returns></returns>
        public static TestResult ReturnLowestConclusiveResultEnumerable(IEnumerable<TestResult> testResults)
        {
            var conclusiveResults = testResults.Where(x => x != TestResult.Inconclusive);
            if (conclusiveResults.Any())
            {
                return conclusiveResults.Min(x => x);
            }
            else
            {
                return TestResult.Inconclusive;
            }
        }

        public static TestResult ReturnLowestConclusiveResult(params TestResult[] testResults)
        {
            return ReturnLowestConclusiveResultEnumerable(testResults);
        }

        public static double ChiSquaredPValue(double[] expectedFrequencies, UInt64[] actual, UInt64 IterationsPerformed, UInt64 minCountPerBin = 5)
        {
            //TODO wrap this dumb thing in a class so that the object can be returned with relevant state for what the worst parts were.
            //walk the array to find min and max
            int lowerIndex = GetLowerIndex(expectedFrequencies, IterationsPerformed, minCountPerBin);
            int upperIndex = GetUpperIndex(expectedFrequencies, IterationsPerformed, minCountPerBin);
            double chiSquared = GetLowerIndexChiSquared(expectedFrequencies, actual, IterationsPerformed, lowerIndex);
            chiSquared += GetHigherIndexChiSquared(expectedFrequencies, actual, IterationsPerformed, upperIndex);
            for (int i = lowerIndex + 1; i < upperIndex; i++)
            {
                double expectedCount = IterationsPerformed * expectedFrequencies[i];
                chiSquared += (expectedCount - actual[i]) * (expectedCount - actual[i]) / (double)expectedCount;
            }
            double pValue = ChiSquaredPValue(upperIndex - lowerIndex, chiSquared);
            return pValue;
        }


        private static double GetHigherIndexChiSquared(double[] expectedFrequencies, UInt64[] actual, UInt64 IterationsPerformed, int upperIndex)
        {
            UInt64 actualCount = 0;
            double expectedFrequency = 0;
            for (int i = upperIndex; i < expectedFrequencies.Length; i++)
            {
                expectedFrequency += expectedFrequencies[i];
                actualCount += actual[i];
            }
            UInt64 expectedCount = Convert.ToUInt64(expectedFrequency * IterationsPerformed);
            var result = (expectedCount - actualCount) * (expectedCount - actualCount) / (double)expectedCount;
            return result;
        }

        private static double GetLowerIndexChiSquared(double[] expectedFrequencies, UInt64[] actual, UInt64 IterationsPerformed, int lowerIndex)
        {
            UInt64 actualCount = 0;
            double expectedFrequency = 0;
            for (int i = 0; i <= lowerIndex; i++)
            {
                expectedFrequency += expectedFrequencies[i];
                actualCount += actual[i];
            }
            UInt64 expectedCount = Convert.ToUInt64(expectedFrequency * IterationsPerformed);
            var result = (expectedCount - actualCount) * (expectedCount - actualCount) / (double)expectedCount;
            return result;
        }
                
        public static double ChiSquaredForPValues(IEnumerable<double> pValues, int numberOfBins = 10)
        {
            var bins = new int[numberOfBins];        //create N bins            
            int index;
            foreach (var value in pValues)          //binning data
            {
                index = (int)(value * numberOfBins);
                if (index == numberOfBins)
                {
                    index--;
                }
                bins[index]++;
            }

            //get the chi-squared stat
            double chiSquaredStatistic = 0;
            double expectedNumber = pValues.Count() / (double)numberOfBins;
            foreach (var observed in bins)
            {
                chiSquaredStatistic += Math.Pow(observed - expectedNumber, 2) / expectedNumber;
            }
            return ChiSquaredPValue(numberOfBins - 1, chiSquaredStatistic);  //get the pvalue            
        }


        /// <summary>
        /// Calculates the upper index where the bin will have at least n elements.
        /// </summary>
        /// <returns></returns>
        private static int GetUpperIndex(double[] expectedFrequencies, UInt64 IterationsPerformed, UInt64 minimumCount)
        {
            int upperIndex = expectedFrequencies.Length - 1;
            while ((expectedFrequencies[upperIndex] * IterationsPerformed < minimumCount) || (expectedFrequencies[upperIndex - 1] * IterationsPerformed < minimumCount))
            {
                upperIndex--;
            }
            return upperIndex;
        }

        /// <summary>
        /// Calculates the lower index where the bin will have at least n elements.
        /// </summary>
        /// <returns></returns>
        private static int GetLowerIndex(double[] expectedFrequencies, UInt64 IterationsPerformed, UInt64 minimumCount)
        {
            int lowerIndex = 0;
            while ((expectedFrequencies[lowerIndex] * IterationsPerformed < minimumCount) || (expectedFrequencies[lowerIndex + 1] * IterationsPerformed < minimumCount))
            {
                lowerIndex++;
            }
            return lowerIndex;
        }

        public static double ChiSquaredPValue(int degreesOfFreedom, double ChiSquaredStatistic)
        {
            return SpecialFunctions.GammaUpperRegularized((double)degreesOfFreedom * 0.5, ChiSquaredStatistic * 0.5);            
        }

        /// <summary>
        /// Arbitrary definitions just to draw some lines.
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="bonferroni">If your pValue is a low one from a number of pValues.</param>
        /// <returns></returns>
        internal static TestResult GetTestResultFromPValue(double pValue, int bonferroni = 1)
        {
            if (pValue > 0.99)
            {
                pValue = 1 - pValue;
            }
            pValue *= bonferroni;
            if (pValue < 1e-9)
            {
                return TestResult.Fail;
            }
            if (pValue < 1e-7)
            {
                return TestResult.VerySuspicious;
            }
            if (pValue < 1e-5)
            {
                return TestResult.Suspicious;
            }
            if (pValue < 1e-3)
            {
                return TestResult.MildlySuspicious;
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Gets GCD with the number of steps
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        internal static ulong GreatestCommonDivisor(ulong a, ulong b, out int steps)
        {
            steps = 0;
            //Put them in consistent order
            if (a < b)
            {
                UInt64 temp = a;
                a = b;
                b = temp;
            }
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;

                steps++;
            }

            if (a == 0)
                return b;
            else
                return a;
        }

        public static int BerlekampMassey(byte[] s)
        {
            int L, N, m, d;
            int n = s.Length;
            byte[] c = new byte[n];
            byte[] b = new byte[n];
            byte[] t = new byte[n];

            //Initialization
            b[0] = c[0] = 1;
            N = L = 0;
            m = -1;

            //Algorithm core
            while (N < n)
            {
                d = s[N];
                for (int i = 1; i <= L; i++)
                    d ^= c[i] & s[N - i];            //(d+=c[i]*s[N-i] mod 2)
                if (d == 1)
                {
                    Array.Copy(c, t, n);    //T(D)<-C(D)
                    for (int i = 0; (i + N - m) < n; i++)
                        c[i + N - m] ^= b[i];
                    if (L <= (N >> 1))
                    {
                        L = N + 1 - L;
                        m = N;
                        Array.Copy(t, b, n);    //B(D)<-T(D)
                    }
                }
                N++;
            }
            return L;
        }
    }
}
