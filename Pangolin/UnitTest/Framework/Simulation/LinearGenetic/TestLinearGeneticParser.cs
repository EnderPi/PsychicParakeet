using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnderPi.Framework.Simulation.LinearGenetic;
using EnderPi.Framework.Random;

namespace UnitTest.Framework.Simulation.LinearGenetic
{
    public class TestLinearGeneticParser
    {
        /// <summary>
        /// Creates an LCG from an RNG species, verifies it matches the LCG built normally.
        /// </summary>
        [Test]
        public void TestLcg()
        {
            string programText = @"multiply s1,3935559000370003845;
                                    add s1,2691343689449507681;
                                    move op,s1;";
            List<Command8099> program = LinearGeneticHelper.Parse(programText);

            var engine = new LinearGeneticEngine(program.ToArray(), new Command8099[0]);
            var lcgGen = new LinearCongruential();

            ulong seed = 5;
            lcgGen.Seed(seed);
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(lcgGen.Next64() == engine.Next64());
            }
        }


        /// <summary>
        /// Creates a XorShift from an RNG species, verifies it matches the XorShift built normally.
        /// </summary>
        [Test]
        public void TestXorShift()
        {            
            string programText = @"move ax,s1;
                                    shiftleft ax,13;
                                    xor s1, ax;
                                    move ax,s1;
                                    shiftright ax, 7;
                                    xor s1, ax;
                                    move ax, s1;
                                    shiftleft ax, 17;
                                    xor s1, ax;
                                    move op,s1;";
            List<Command8099> program = LinearGeneticHelper.Parse(programText);

            var engine = new LinearGeneticEngine(program.ToArray(), new Command8099[] { new OrConstant(0, 1)});
            
            var xorshiftGen = new XorShift();

            ulong seed = 4;
            xorshiftGen.Seed(seed);
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(xorshiftGen.Next64() == engine.Next64());
            }
        }





    }
}
