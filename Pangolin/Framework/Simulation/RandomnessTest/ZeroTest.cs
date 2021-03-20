using EnderPi.Framework.Services;
using System.Collections.Generic;
using System.Linq;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class ZeroTest : IIncrementalRandomTest
    {
        private TestResult _result;
        private Queue<ulong> _queue;
        public TestResult Result => _result;

        public int TestsPassed => _result == TestResult.Fail ? 0 : 1;

        /// <summary>
        /// Literally just fails if the same value appears 10 times in the first 50 values.
        /// </summary>
        /// <param name="detailed"></param>
        public void CalculateResult(bool detailed)
        {
            if (_queue.Count == 50)
            {
                var countOfDupes = _queue.GroupBy(x => x).OrderByDescending(y => y.Count()).First().Count();
                if (countOfDupes > 10)
                {
                    _result = TestResult.Fail;
                }
            }
        }

        public void Initialize()
        {
            _result = TestResult.Inconclusive;
            _queue = new Queue<ulong>(55);
        }

        public void Process(ulong randomNumber)
        {
            _queue.Enqueue(randomNumber);
            while (_queue.Count > 50)
            {
                _queue.Dequeue();
            }            
        }

        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {
            
        }
    }
}
