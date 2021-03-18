using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using System.Linq;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public class BirthdayRotatedTest : IIncrementalRandomTest
    {
        private BirthdayTest[] _tests;

        private TestResult _overallResult;

        public TestResult Result => _overallResult;

        public int TestsPassed { get { return _tests.Where(x => x.Result != TestResult.Fail).Count(); } }

        public void CalculateResult(bool detailed)
        {
            TestResult[] testResults = new TestResult[64];
            //get the results...
            for (int i = 0; i < 64; i++)
            {
                _tests[i].CalculateResult(detailed);
                testResults[i] = _tests[i].Result;
            }

            //So these tests are not very independent.  Best to just take the minimum, doesn't make sense to do P calculations.
            _overallResult = TestHelper.ReturnLowestConclusiveResult(testResults);
        }

        public void Initialize()
        {
            _overallResult = TestResult.Inconclusive;
            _tests = new BirthdayTest[64];
            for (int i = 0; i < 64; i++)
            {
                _tests[i] = new BirthdayTest();
            }
        }

        public void Process(ulong randomNumber)
        {
            for (int i = 0; i < 64; i++)
            {
                _tests[i].Process(RandomHelper.RotateRight(randomNumber, i));
            }
        }

        public void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState)
        {
            CalculateResult(true);
            if (persistState)
            {
                //write records to the daterbase.
                //so we need a gcdrotatedtest table
                //plus some children tables.
            }

        }
    }
}
