using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.RandomnessTest;
using NUnit.Framework;

namespace UnitTest.Framework.Simulation.RandomnessTest
{
    public class TestMaurer
    {
        /// <summary>
        /// So this test creates a "good" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng()
        {
            var engine = new Sha256();
            var maurerTest = new MaurerByteTest();
            maurerTest.Initialize();
            for (int i = 0; i < 100000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }

        /// <summary>
        /// So this test creates a "good" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng16()
        {
            var engine = new Sha256();
            var maurerTest = new Maurer16BitTest();
            maurerTest.Initialize();
            for (int i = 0; i < 20000000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }


        /// <summary>
        /// So this test creates a "bad" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestBadRng()
        {
            var engine = new SequentialEngine();
            var maurerTest = new MaurerByteTest();
            maurerTest.Initialize();
            for (int i = 0; i < 100000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }

        /// <summary>
        /// So this test creates a "good" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestBadRng16()
        {
            var engine = new LinearCongruential();
            var maurerTest = new Maurer16BitTest();
            maurerTest.Initialize();
            for (int i = 0; i < 20000000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }

        /// <summary>
        /// So this test creates a "good" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng16Romul()
        {
            var engine = new Romul();
            var maurerTest = new Maurer16BitTest();
            maurerTest.Initialize();
            for (int i = 0; i < 20000000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result == TestResult.Pass);
        }

        /// <summary>
        /// So this test creates a "good" engine, then runs a Maurer test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng24()
        {
            var engine = new Sha256();
            var maurerTest = new Maurer24BitTest();
            maurerTest.Initialize();
            for (int i = 0; i < 2000000000; i++)
            {
                maurerTest.Process(engine.Next64());
            }
            maurerTest.CalculateResult(true);
            var result = maurerTest.Result;
            Assert.IsTrue(result == TestResult.Pass);
        }

    }
}
