using EnderPi.Framework.Services;
using System;
using System.Linq;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class BitwiseLinear : IIncrementalRandomTest
    {
        private byte[][] _bits;
        private int _counter;
        private int constant = 1000;
        private TestResult _result;
        private TestResult[] _results;
        private bool _calculated;

        public TestResult Result => _result;

        public int TestsPassed => _results.Count(x=>x == TestResult.Pass);

        public void CalculateResult(bool detailed)
        {
            if (_counter < constant || _calculated)
            {
                return;
            }
            int[] complexity = new int[64];
            for (int i = 0; i < 64; i++)
            {
                complexity[i] = BerlekampMassey(_bits[i]);
                _results[i] = complexity[i] < (constant / 3) ? TestResult.Fail : TestResult.Pass; 
            }
            _result = TestHelper.ReturnLowestConclusiveResult(_results);
            _calculated = true;
        }

        public void Initialize()
        {
            _calculated = false;
            _bits = new byte[64][];
            for (int i=0; i < 64; i++)
            {
                _bits[i] = new byte[constant];
            }
            _result = TestResult.Inconclusive;
            _results = new TestResult[64];
        }

        public void Process(ulong randomNumber)
        {
            if (_counter == constant)
            {
                return;
            }
            for (int i = 0; i < 64; i++)
            {
                _bits[i][_counter] = Convert.ToByte((randomNumber >> i) & 1UL);
            }
            _counter++;            
        }

        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {
            
        }

        private static int BerlekampMassey(byte[] s)
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
