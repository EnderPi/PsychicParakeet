using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Extensions;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization;
using EnderPi.Framework.Simulation.Genetic.Nodes;
using EnderPi.Framework.Logging;

namespace EnderPi.Framework.Simulation.Genetic
{
    /// <summary>
    /// A long running task that does genetic programming for random number generators.  Handful of these classes are pretty tightly coupled.
    /// </summary>
    [Serializable]
    [DataContract(Name = "GeneticRngBreeding", Namespace = "EnderPi")]
    public class GeneticRngBreeding : LongRunningTask
    {
        /// <summary>
        /// The specimens in the current generation.
        /// </summary>
        [DataMember]
        private List<RngSpecies> _specimens;
        /// <summary>
        /// The specimens in the next generation.  Maybe this doesn't need to be state.
        /// TODO consider if this needs to be state.
        /// </summary>
        [DataMember]
        private List<RngSpecies> _specimensNextGeneration;
        /// <summary>
        /// The random engine used for the genetic programming.
        /// </summary>
        [DataMember]
        private Engine _randomEngine;

        /// <summary>
        /// The current generation of the genetic algorithm.
        /// </summary>
        [DataMember]
        private int _generation;


        [DataMember]
        private GeneticParameters _parameters;

        public int Generations { get { return _generation; } }


        /// <summary>
        /// Feels like just for testing right now.
        /// </summary>
        public RngSpecies Biggest { get { return _specimens.OrderByDescending(x=>x.NodeCount).FirstOrDefault(); } }

        public RngSpecies Best { get { return _specimens.OrderByDescending(x => x.Fitness).FirstOrDefault(); } }


        public delegate void GeneticEventHandler(object sender, GeneticEventArgs e);

        public event GeneticEventHandler GenerationFinished;

        private void OnGenerationFinished(ServiceProvider provider)
        {
            GeneticEventArgs e = new GeneticEventArgs();
            Logger logger = provider.GetService<Logger>();
            try
            {
                e.Generation = _generation;
                e.ThisGeneration = _specimens.DeepCopy();
                var engine = e.ThisGeneration[0].GetEngine() as Engine;
                engine.Seed(e.ThisGeneration[0].Seed);
                e.Image = engine.GetBitMap(4096);                
            }            
            catch (Exception ex)
            {
                logger.Log($"Error {ex.ToString()}", LoggingLevel.Error);
            }
            
            Threading.Threading.ExecuteWithoutThrowing(() => GenerationFinished?.Invoke(this, e), logger);
        }

        /// <summary>
        /// Basic constructor.  No real initial state.
        /// </summary>        
        public GeneticRngBreeding(GeneticParameters parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Constructor if you want to pass in some starting species.
        /// </summary>
        /// <param name="initialSpecies"></param>
        public GeneticRngBreeding(List<RngSpecies> initialSpecies)
        {
            _specimens = initialSpecies;            
        }

        /// <summary>
        /// Initializes the random number generator, and the initial generation, which will wind up getting filled with lots of boring stuff.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="backgroundTaskId"></param>
        /// <param name="persistState"></param>
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            _generation = 0;
            _randomEngine = new Sha256();
            IConfigurationDataAccess configDataAccess = provider.GetService<IConfigurationDataAccess>();
            int speciesPerGeneration = configDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticGenerationSize, 128);
            if (_specimens == null)
            {
                _specimens = new List<RngSpecies>(speciesPerGeneration);
            }
            while (_specimens.Count < speciesPerGeneration)
            {
                int i = _parameters.Mode == ConstraintMode.None ? 1 : 3;
                var species = new RngSpecies(_parameters.Mode);
                for (; i <= 3; i++)
                {
                    var treeToMutate = species.GetTreeRoot(i);
                    AddNode(treeToMutate);                    
                }
                AddSpecies(_specimens, species);                
            }
            foreach (var specimen in _specimens)
            {
                specimen.SetNodeAge(0);
                EvaluateFitness(specimen, token, provider, backgroundTaskId);
                if (token.IsCancellationRequested) break;
            }
            _specimens = _specimens.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
            OnGenerationFinished(provider);
        }

        /// <summary>
        /// Adds a species to the list, persists it to the underlying table.
        /// </summary>
        /// <param name="specimens"></param>
        /// <param name="rngSpecies"></param>
        private void AddSpecies(List<RngSpecies> specimens, RngSpecies rngSpecies)
        {
            rngSpecies.Generation = _generation;            
            specimens.Add(rngSpecies);
            //write to the table, get the specimen ID.
            //rngSpecies.SpecimenId = 

            
        }

        private IComparer<RngSpecies> GetSpeciesComparer()
        {
            switch (_parameters.CostMode)
            {
                case GeneticCostMode.PreferLowerNodeCount:
                    return new SpeciesComparerNodes();
                case GeneticCostMode.PreferLowerCost:
                    return new SpeciesComparerCost();
            }
            return new SpeciesComparerNodes();
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            bool converged = false;
            while (!token.IsCancellationRequested && !converged)
            {
                _generation++;
                _randomEngine.Seed(Engine.Crypto64());
                //INJECT ANY FROM DATERBASE TABLE IF THEY EXIST
                
                
                _specimensNextGeneration = new List<RngSpecies>();
                _specimensNextGeneration.AddRange(EliteSpecimens(provider));
                SelectAndBreed(provider);
                _specimens = _specimensNextGeneration;  //replacing....
                foreach (var specimen in _specimens)
                {
                    EvaluateFitness(specimen, token, provider, backgroundTaskId);
                    if (token.IsCancellationRequested) break;
                }
                _specimens = _specimens.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
                double averageFitness = _specimens.Average(x => x.Fitness);
                double medianFitness = _specimens[_specimens.Count / 2].Fitness;
                
                //write all to a table.
                

                converged = (Best.Generation + 10) < _generation;
                OnGenerationFinished(provider);
                //save if necessary
                //report progress?
            }            
        }




        private List<RngSpecies> EliteSpecimens(ServiceProvider provider)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            int specimensToTake = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticEliteSize, 16);
            return _specimens.Take(specimensToTake).ToList();
        }

        private void SelectAndBreed(ServiceProvider provider)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            int specimensPerGeneration = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticGenerationSize, 128);

            while (_specimensNextGeneration.Count < specimensPerGeneration)
            {
                try
                {
                    RngSpecies dad = SelectRandomFitSpecimen(provider);
                    RngSpecies mom = SelectRandomFitSpecimen(provider);
                    List<RngSpecies> children = Crossover(dad, mom);
                    MaybeMutate(provider, children);
                    FoldConstants(children, provider);
                    _specimensNextGeneration.AddRange(children);
                }
                catch (Exception ex)
                {
                    var logger = provider.GetService<Logger>();
                    LogDetails details = new LogDetails();
                    details.AddDetail("Exception", ex.ToString());
                    logger.Log("Error breeding specimens!", LoggingLevel.Error, details);
                }
            }            
        }

        /// <summary>
        /// Folds constants of the trees.  Reverts on exception, very much a work in progress.
        /// </summary>
        /// <remarks>
        /// Problematic because it may fold things into something greater than a long.
        /// </remarks>
        /// <param name="children"></param>
        /// <param name="provider"></param>
        private void FoldConstants(List<RngSpecies> children, ServiceProvider provider)
        {
            for (int i=0; i < children.Count; i++)
            {
                var child = children[i];
                RngSpecies backup = child.DeepCopy();
                TreeNode node = null;
                try
                {
                    while (HasAConstantFoldableNode(child, out var root, out node))
                    {
                        FoldAConstantNode(root, node);
                    }
                }
                catch (Exception ex)
                {
                    //folding failed, revert to backup
                    children[i] = backup;
                    var logger = provider.GetService<Logger>();
                    LogDetails details = new LogDetails();
                    if (node != null)
                    {
                        details.AddDetail("Node expression", node.Evaluate());
                    }
                    details.AddDetail("Exception", ex.ToString());
                    logger.Log("Failure trying to fold constant!", LoggingLevel.Error, details);
                }
            }
        }

        private void FoldAConstantNode(TreeNode root, TreeNode node)
        {
            ConstantNode constantNode = new ConstantNode(node.Fold());
            root.ReplaceAllChildReferences(node, constantNode);
        }

        private bool HasAConstantFoldableNode(RngSpecies child, out TreeNode root, out TreeNode foldableNode)
        {
            root = null;
            foldableNode = null;
            for (int i=1; i<=5; i++ )
            {
                root = child.GetTreeRoot(i);
                foreach(var childNode in root.GetDescendants())
                {
                    if (childNode.IsFoldable())
                    {
                        foldableNode = childNode;
                        return true;
                    }
                }

            }
            return false;
        }

        private void MaybeMutate(ServiceProvider provider, List<RngSpecies> children)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            double mutationProbability = configurationDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticMutationChance, (double)1.0/16);
            double xmenProbability = configurationDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticXMenChance, (double)1.0 / 32);

            foreach (var child in children)
            {
                //small chance of mega mutation, like 8 straight adds.
                if (_randomEngine.NextDoubleInclusive() < xmenProbability)
                {
                    var treeToMutateRoot = child.GetTreeRoot(PickTreeToModify());
                    for (int i = 0; i < 8; i++)
                    {
                        AddNode(treeToMutateRoot);
                    }
                    continue;
                }

                while (_randomEngine.NextDoubleInclusive() < mutationProbability)   
                {
                    var treeToMutateRoot = child.GetTreeRoot(PickTreeToModify());
                    
                    uint choice = _randomEngine.Next32(1, 3);
                    switch (choice)
                    {
                        case 1:                            
                            AddNode(treeToMutateRoot);
                            break;

                        case 2:
                            if (treeToMutateRoot.GetDescendantsNodeCount() == 1)
                            {
                                AddNode(treeToMutateRoot);
                            }
                            else
                            {
                                DeleteANode(treeToMutateRoot);
                            }                            
                            break;
                        
                        case 3:
                            ChangeANode(treeToMutateRoot);
                            break;
                        case 4:
                            MakeTopALoop(treeToMutateRoot);
                            break;
                                                
                    }                    
                }
            }            
        }

        private void MakeTopALoop(TreeNode treeToMutateRoot)
        {
            var childToReplace = treeToMutateRoot.GetFirstChild();
            var loopNode = new LoopNode(childToReplace, _randomEngine.NextInt(2, 8));
            treeToMutateRoot.ReplaceFirstChild(loopNode);
            
        }

        private void ChangeANode(TreeNode treeToMutateRoot)
        {
            //change a node
            //find a random node
            //heavily favor changing a constant by flipping a bit
            //if it's a constant, change the constant, or change the constant to a state.
            //if it's a state node, change it to the other state, or change it to a constant if that doesn't invalidate the parent.
            //if it's a binary node, flip the type.
            var descendants = treeToMutateRoot.GetDescendants().ToList();
            var nodeToMutate = _randomEngine.GetRandomElement(descendants);
            if (nodeToMutate is ConstantNode constantNode)
            {
                ulong x = _randomEngine.FlipRandomBit(constantNode.Value);
                if (x > long.MaxValue)
                {
                    x ^= (1UL << 63);
                }
                constantNode.Value = x;                
                //flip a bit
            }
            else if (nodeToMutate is StateOneNode)
            {
                var newNode = _randomEngine.PickRandomElement<TreeNode>(new StateTwoNode(), new ConstantNode(_randomEngine.Next64(0, long.MaxValue)));
                treeToMutateRoot.ReplaceAllChildReferences(nodeToMutate, newNode);
            }
            else if (nodeToMutate is StateTwoNode)
            {
                var newNode = _randomEngine.PickRandomElement<TreeNode>(new StateOneNode(), new ConstantNode(_randomEngine.Next64(0, long.MaxValue)));
                treeToMutateRoot.ReplaceAllChildReferences(nodeToMutate, newNode);
            }
            else if (nodeToMutate is SeedNode)
            {
                //not much to do here
            }
            //else if (nodeToMutate is BinaryNode)
            //{
                
            //}
            //else if (nodeToMutate is UnaryNode)
            //{

            //}
            
        }

        private void DeleteANode(TreeNode treeToMutateRoot)
        {
            var leafNodes = treeToMutateRoot.GetDescendants().Where(x => x.IsLeafNode).ToList();
            //TODO TEST THIS CODE
            var levelOneNodes = treeToMutateRoot.GetDescendants().Where(x => leafNodes.Where(y=>x.IsChild(y)).Any()).ToList();
            var nodeToDelete = _randomEngine.GetRandomElement(levelOneNodes);
            TreeNode replacementNode;
            var childNodes = nodeToDelete.GetDescendants().ToList();
            var childStateSeedNodes = childNodes.Where(x => x is StateOneNode || x is StateTwoNode || x is SeedNode).ToList();
            if (childStateSeedNodes.Count > 0)
            {
                replacementNode = _randomEngine.GetRandomElement(childStateSeedNodes);
            }
            else
            {
                replacementNode = _randomEngine.GetRandomElement(childNodes);
            }
            treeToMutateRoot.ReplaceAllChildReferences(nodeToDelete, replacementNode);            
        }

        private void AddNode(TreeNode treeToMutateRoot)
        {
            var leafNodes = treeToMutateRoot.GetDescendants().Where(x => x.IsLeafNode).ToList();
            //pick a random leaf
            var randomLeaf = _randomEngine.GetRandomElement(leafNodes);
            TreeNode secondNode;

            if (treeToMutateRoot is SeedRootNode)
            {
                if (randomLeaf is SeedNode)
                {
                    secondNode = new ConstantNode(_randomEngine.Next64(0, long.MaxValue));
                }
                else
                {
                    secondNode = new SeedNode();
                }
            }
            else
            {
                if (randomLeaf is StateOneNode || randomLeaf is StateTwoNode)
                {
                    secondNode = new ConstantNode(_randomEngine.Next64(0, long.MaxValue));      //or a state node, right?
                }
                else
                {
                    secondNode = _randomEngine.PickRandomElement<TreeNode>(new StateOneNode(), new StateTwoNode());
                }
            }

            secondNode.GenerationOfOrigin = _generation;
            var parentNodes = treeToMutateRoot.GetDescendants().Where(x => x.IsChild(randomLeaf)).ToList();
            TreeNode newNode = MakeNewBinaryNode(randomLeaf, secondNode);
            treeToMutateRoot.ReplaceAllChildReferences(randomLeaf, newNode);
        }

        private TreeNode MakeNewBinaryNode(TreeNode randomLeaf, TreeNode secondNode)
        {            
            //List<TreeNode> possibleNodes = new List<TreeNode>(16);
            //if (_parameters.AllowAdditionNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new AdditionNode(randomLeaf, secondNode), new AdditionNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowSubtractionNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new SubtractNode(randomLeaf, secondNode), new SubtractNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowMultiplicationNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new MultiplicationNode(randomLeaf, secondNode), new MultiplicationNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowDivisionNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new DivideNode(randomLeaf, secondNode), new DivideNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowOrNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new OrNode(randomLeaf, secondNode), new OrNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowXorNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new XorNode(randomLeaf, secondNode), new XorNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowAndNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new AndNode(randomLeaf, secondNode), new AndNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowLeftShiftNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new LeftShiftNode(randomLeaf, secondNode), new LeftShiftNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowRightShiftNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new RightShiftNode(randomLeaf, secondNode), new RightShiftNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowRotateLeftNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new RotateLeftNode(randomLeaf, secondNode), new RotateLeftNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowRotateRightNodes)
            //{
            //    possibleNodes.Add(_randomEngine.PickRandomElement(new RotateRightNode(randomLeaf, secondNode), new RotateRightNode(secondNode, randomLeaf)));
            //}
            //if (_parameters.AllowNotNodes)
            //{
            //    possibleNodes.Add(new NotNode(randomLeaf));
            //}
            //TreeNode result = _randomEngine.GetRandomElement(possibleNodes);
            //result.GenerationOfOrigin = _generation;
            //return result;


            TreeNode result = null;
            switch (_randomEngine.NextInt(0, 11))
            {
                case 0:
                    result = _randomEngine.PickRandomElement(new AdditionNode(randomLeaf, secondNode), new AdditionNode(secondNode, randomLeaf));
                    break;
                case 1:
                    result = _randomEngine.PickRandomElement(new SubtractNode(randomLeaf, secondNode), new SubtractNode(secondNode, randomLeaf));
                    break;
                case 2:
                    result = _randomEngine.PickRandomElement(new MultiplicationNode(randomLeaf, secondNode), new MultiplicationNode(secondNode, randomLeaf));
                    break;
                case 3:
                    result = _randomEngine.PickRandomElement(new DivideNode(randomLeaf, secondNode), new DivideNode(secondNode, randomLeaf));
                    break;
                case 4:
                    result = _randomEngine.PickRandomElement(new OrNode(randomLeaf, secondNode), new OrNode(secondNode, randomLeaf));
                    break;
                case 5:
                    result = _randomEngine.PickRandomElement(new XorNode(randomLeaf, secondNode), new XorNode(secondNode, randomLeaf));
                    break;
                case 6:
                    result = _randomEngine.PickRandomElement(new AndNode(randomLeaf, secondNode), new AndNode(secondNode, randomLeaf));
                    break;
                case 7:
                    result = _randomEngine.PickRandomElement(new LeftShiftNode(randomLeaf, secondNode), new LeftShiftNode(secondNode, randomLeaf));
                    break;
                case 8:
                    result = _randomEngine.PickRandomElement(new RightShiftNode(randomLeaf, secondNode), new RightShiftNode(secondNode, randomLeaf));
                    break;
                case 9:
                    result = _randomEngine.PickRandomElement(new RotateLeftNode(randomLeaf, secondNode), new RotateLeftNode(secondNode, randomLeaf));
                    break;
                case 10:
                    result = _randomEngine.PickRandomElement(new RotateRightNode(randomLeaf, secondNode), new RotateRightNode(secondNode, randomLeaf));
                    break;
                case 11:
                    result = new NotNode(randomLeaf);
                    break;
                case 12:
                    result = new LoopNode(randomLeaf, 4);
                    break;
                default:
                    result = _randomEngine.PickRandomElement(new AdditionNode(randomLeaf, secondNode), new AdditionNode(secondNode, randomLeaf));
                    break;
            }
            result.GenerationOfOrigin = _generation;
            return result;

        }

        private RngSpecies SelectRandomFitSpecimen(ServiceProvider provider)
        {
            //coding tournament selection\
            var globalsettingsDataAccess = provider.GetService<IConfigurationDataAccess>();
            //TODO cache this, or inject a cached provider
            int tournamentSize = globalsettingsDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticTournamentPopulation, 8);
            double probability = globalsettingsDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticTournamentProbability, 0.9);
            //select tournamanet size specimens.
            var specimenlist = new List<RngSpecies>(_specimens);
            _randomEngine.Shuffle(specimenlist);
            specimenlist = specimenlist.Take(tournamentSize).ToList();
            specimenlist = specimenlist.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
            //now pick from the list.
            RngSpecies selectedSpecimen = SelectByTournament(specimenlist, probability);
            return selectedSpecimen;
        }

        private RngSpecies SelectByTournament(List<RngSpecies> specimenlist, double p)
        {
            double[] probs = new double[specimenlist.Count];
            double[] cumulativeprobs = new double[specimenlist.Count];
            double totalProb = 0;
            for (int i=0; i < probs.Length; i++)
            {
                probs[i] = p * Math.Pow(1 - p, i);                
                totalProb += probs[i];
            }
            probs[0] += 1 - totalProb;
            cumulativeprobs[0] = probs[0];
            for (int i=1; i < cumulativeprobs.Length; i++)
            {
                cumulativeprobs[i] = cumulativeprobs[i - 1] + probs[i];
            }
            double randomdouble = _randomEngine.NextDoubleInclusive();
            int index;
            for (index = 0; cumulativeprobs[index] < randomdouble && index < cumulativeprobs.Length; index++) ;
            return specimenlist[index];
        }

        /// <summary>
        /// Crosses over mom and dad, returns two kids.
        /// </summary>
        /// <param name="dad"></param>
        /// <param name="mom"></param>
        /// <returns></returns>
        private List<RngSpecies> Crossover(RngSpecies dad, RngSpecies mom)
        {            
            RngSpecies son = dad.DeepCopy();
            RngSpecies daughter = mom.DeepCopy();
            son.Fitness = 0;
            daughter.Fitness = 0;
            son.Generation = _generation;
            son.Name = NameGenerator.GetName();
            daughter.Name = NameGenerator.GetName();
            daughter.Generation = _generation;
            son.Birthday = daughter.Birthday = DateTime.Now;
            //pick a tree, favor state or output heavily, as seed doesn't matter much
            int treeToModify = _parameters.Mode == ConstraintMode.None ? 1 : 3;
            for (; treeToModify <= 5; treeToModify++)
            {
                var sonTreeRoot = son.GetTreeRoot(treeToModify);
                var daughterTreeRoot = daughter.GetTreeRoot(treeToModify);
                TreeNode sonTreeNode = PickRandomTreeNode(sonTreeRoot);
                TreeNode daughterTreeNode = PickRandomTreeNode(daughterTreeRoot);
                sonTreeRoot.ReplaceAllChildReferences(sonTreeNode, daughterTreeNode);
                daughterTreeRoot.ReplaceAllChildReferences(daughterTreeNode, sonTreeNode);
            }
            return new List<RngSpecies>() { son, daughter };
        }

        private TreeNode PickRandomTreeNode(TreeNode sonTreeRoot)
        {
            var childrenNodes = sonTreeRoot.GetDescendants();
            int count = childrenNodes.Count;
            if (count == 1)
            {
                return childrenNodes[0];
            }            
            int index = Convert.ToInt32(_randomEngine.Next32(0, (uint)(count - 1)));
            return childrenNodes[index];
        }

        private int PickTreeToModify()
        {
            if (_parameters.Mode == ConstraintMode.None)
            {
                var number = _randomEngine.Next32(1, 32);
                if (number <= 10)
                {
                    return 1;
                }
                if (number <= 20)
                {
                    return 2;
                }
                if (number <= 30)
                {
                    return 3;
                }
                if (number == 31)
                {
                    return 4;
                }
                if (number == 32)
                {
                    return 5;
                }
            }
            else
            {
                return 3;
            }
            return 3;
        }

        private void EvaluateFitness(RngSpecies specimen, CancellationToken token, ServiceProvider provider, int backgroundTaskId)
        {
            RandomnessSimulation randomTest = null;
            if (specimen.Fitness != 0)
            {
                return;
            }
            try
            {
                var engine = specimen.GetEngine();
                specimen.Seed = Engine.Crypto64();
                randomTest = new RandomnessSimulation(_parameters.Level, engine, specimen.Seed);
                randomTest.Start(token, provider, backgroundTaskId, false);
                specimen.Fitness = randomTest.Iterations;
                specimen.TestsPassed = randomTest.TestsPassed;
            }
            catch (DivideByZeroException)
            {
                if (randomTest != null)
                {
                    specimen.Fitness = -randomTest.Iterations;
                }
                else
                {
                    specimen.Fitness = -1;
                }
            }
            catch (Exception ex)
            {
                //An exception means there's something fatal, like a divide by zero.
                specimen.Fitness = -1;
                var logger = provider.GetService<Logger>();
                var details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                logger.Log("Error trying to evaluate fitness!", LoggingLevel.Information, details);
            }
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //throw new NotImplementedException();
        }
    }
}
