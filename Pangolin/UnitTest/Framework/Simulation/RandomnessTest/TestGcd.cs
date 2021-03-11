using NUnit.Framework;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;

namespace UnitTest.Framework.Simulation.RandomnessTest
{
    /// <summary>
    /// Some unit tests for the GCD framework.
    /// </summary>
    public class TestGcd
    {
        /// <summary>
        /// So this test creates a "good" engine, then runs a Gcd test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng()
        {
            var engine = new Sha256();
            var gcdTest = new GcdTest();
            gcdTest.Initialize();
            for (int i=0; i< 100000; i++)
            {
                gcdTest.Process(engine.Next64());
            }
            gcdTest.CalculateResult(true);
            var result = gcdTest.Result;
            Assert.IsTrue(result != TestResult.Fail);            
        }

        /// <summary>
        /// So this test creates a "bad" engine, then runs a Gcd test and verifies it fails.
        /// </summary>
        [Test]
        public void TestBadRng()
        {
            var engine = new LinearCongruential();
            var gcdTest = new GcdTest();
            gcdTest.Initialize();
            for (int i = 0; i < 100000; i++)
            {
                gcdTest.Process(engine.Next64());
            }
            gcdTest.CalculateResult(true);
            var result = gcdTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }


    }
}
