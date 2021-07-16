using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;

namespace UnitTest.Framework.Simulation.Randomness16Test
{
    public class TestGcd16
    {
        /// <summary>
        /// So this test creates a "good" engine, then runs a Gcd test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng()
        {
            var engine = new Sha16Bit();
            var gcdTest = new Gcd16Test();
            gcdTest.Initialize();
            for (int i = 0; i < 100000; i++)
            {
                gcdTest.Process(engine.Next16());
            }
            gcdTest.CalculateResult(true);
            var result = gcdTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }


        /// <summary>
        /// So this test creates a "good" engine, then runs a Gcd test and verifies it passes.
        /// </summary>
        [Test]
        public void TestRomulRng()
        {
            var engine = new Romul16();
            var gcdTest = new Gcd16Test();
            gcdTest.Initialize();
            for (int i = 0; i < 1000; i++)
            {
                gcdTest.Process(engine.Next16());
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
            var engine = new LinearCongruential16();
            var gcdTest = new Gcd16Test();
            gcdTest.Initialize();
            for (int i = 0; i < 100000; i++)
            {
                gcdTest.Process(engine.Next16());
            }
            gcdTest.CalculateResult(true);
            var result = gcdTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }
    }
}
