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

        public void CalculateResult(bool detailed)
        {
            if (_queue.Count == 50 && _queue.All(x=>x==0))
            {
                _result = TestResult.Fail;
            }
        }

        public void Initialize()
        {
            _result = TestResult.Inconclusive;
            _queue = new Queue<ulong>(50);
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
