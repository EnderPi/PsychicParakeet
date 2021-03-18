using System.Diagnostics;
using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.Genetic;
using NUnit.Framework;

namespace UnitTest.Framework.Simulation.Genetic
{
    public class TestRngSpecies
    {
        /// <summary>
        /// Creates an LCG from an RNG species, verifies it matches the LCG built normally.
        /// </summary>
        [Test]
        public void TestLcg()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);
            TreeNode stateOneNode = new AdditionNode(new MultiplicationNode(new StateOneNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681));
            var lcgSpecies = new RngSpecies(stateOneNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = lcgSpecies.GetEngine();
            var lcgGen = new LinearCongruential();

            ulong seed = 4;
            lcgGen.Seed(seed);
            engine.Seed(seed);

            for (int i=0; i<10; i++)
            {
                Assert.IsTrue(lcgGen.Next64() == engine.Next64());
            }        
        }

        /// <summary>
        /// Creates an LCG from an RNG species, verifies it matches the LCG built normally.
        /// </summary>
        [Test]
        public void TestXorShift()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);

            TreeNode leftshift = new LeftShiftNode(new StateOneNode(), new ConstantNode(13));
            TreeNode firstXorNode = new XorNode(new StateOneNode(), leftshift);
            TreeNode secondXorNode = new XorNode(firstXorNode, new RightShiftNode(firstXorNode, new ConstantNode(7)));
            TreeNode thirdXorNode = new XorNode(secondXorNode, new LeftShiftNode(secondXorNode, new ConstantNode(17)));

                        
            var xorShiftSpecies = new RngSpecies(thirdXorNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = xorShiftSpecies.GetEngine();
            var xorshiftGen = new XorShift();

            ulong seed = 4;
            xorshiftGen.Seed(seed);
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(xorshiftGen.Next64() == engine.Next64());
            }
        }



        [Test]
        public void SpeedTestLcg()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);
            TreeNode stateOneNode = new AdditionNode(new MultiplicationNode(new StateOneNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681));
            var lcgSpecies = new RngSpecies(stateOneNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = lcgSpecies.GetEngine();
            
            ulong seed = 4;
            engine.Seed(seed);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                engine.Next64();
            }
            stopwatch.Stop();

            var lcgGen = new LinearCongruential();
            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                lcgGen.Next64();
            }
            stopwatch2.Stop();



            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000000);
            Assert.IsTrue(stopwatch2.ElapsedMilliseconds < 1000000);
        }

        [Test]
        public void SpeedTestXorShift()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);

            TreeNode leftshift = new LeftShiftNode(new StateOneNode(), new ConstantNode(13));
            TreeNode firstXorNode = new XorNode(new StateOneNode(), leftshift);
            TreeNode secondXorNode = new XorNode(firstXorNode, new RightShiftNode(firstXorNode, new ConstantNode(7)));
            TreeNode thirdXorNode = new XorNode(secondXorNode, new LeftShiftNode(secondXorNode, new ConstantNode(17)));


            var xorshiftSpecies = new RngSpecies(thirdXorNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = xorshiftSpecies.GetEngine();

            ulong seed = 4;
            engine.Seed(seed);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                engine.Next64();
            }
            stopwatch.Stop();

            var xorgen = new XorShift();
            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                xorgen.Next64();
            }
            stopwatch2.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000000);
            Assert.IsTrue(stopwatch2.ElapsedMilliseconds < 1000000);
        }

        [Test]
        public void TestRotateLeft()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);
            TreeNode stateOneNode = new RotateLeftNode(new StateOneNode(), new ConstantNode(1));
            var lcgSpecies = new RngSpecies(stateOneNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = lcgSpecies.GetEngine();
            
            ulong seed = 4;
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(engine.Next64() != 0);
            }
        }

        [Test]
        public void TestRotateRight()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);
            TreeNode stateOneNode = new RotateRightNode(new StateOneNode(), new ConstantNode(9999999999999999));
            var lcgSpecies = new RngSpecies(stateOneNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = lcgSpecies.GetEngine();

            ulong seed = 4;
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(engine.Next64() != 0);
            }
        }

        [Test]
        public void TestRotateRight2()
        {
            TreeNode output = new StateOneNode();
            TreeNode seedOneNode = new OrNode(new SeedNode(), new ConstantNode(1));
            TreeNode seedTwoNode = new ConstantNode(1);
            TreeNode stateTwoNode = new ConstantNode(2);
            TreeNode stateOneNode = new RotateRightNode(new ConstantNode(9999999999999999), new StateOneNode());
            var lcgSpecies = new RngSpecies(stateOneNode, stateTwoNode, output, seedOneNode, seedTwoNode);
            var engine = lcgSpecies.GetEngine();

            ulong seed = 4;
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(engine.Next64() != 0);
            }
        }
        
    }
}
