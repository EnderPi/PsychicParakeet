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

        private int _simulationId;

        /// <summary>
        /// Feels like just for testing right now.
        /// </summary>
        public RngSpecies Biggest { get { return _specimens.OrderByDescending(x=>x.NodeCount).FirstOrDefault(); } }

        public RngSpecies Best { get { return _specimens.OrderByDescending(x => x.Fitness).FirstOrDefault(); } }

        private int _iteration;

        public delegate void GeneticEventHandler(object sender, GeneticEventArgs e);

        public event GeneticEventHandler GenerationFinished;

        private void OnGenerationFinished(ServiceProvider provider)
        {
            GeneticEventArgs e = new GeneticEventArgs();
            Logger logger = provider.GetService<Logger>();
            try
            {
                e.Generation = _generation;
                e.Iteration = _iteration;
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
            _randomEngine = new Sha256();
            var geneticSimulationDataAccess = provider.GetService<IGeneticSimulationDataAccess>();
            _simulationId = geneticSimulationDataAccess.CreateGeneticSimulation(_parameters);
            _iteration = 0;
        }

        private void InitializeGeneration(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            _generation = 0;
            IConfigurationDataAccess configDataAccess = provider.GetService<IConfigurationDataAccess>();
            ISpeciesNameDataAccess speciesNameDataAccess = provider.GetService<ISpeciesNameDataAccess>();
            int speciesPerGeneration = configDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticGenerationSize, 128);
            _specimens = new List<RngSpecies>(speciesPerGeneration);
            while (_specimens.Count < speciesPerGeneration)
            {
                var species = new RngSpecies(_parameters.ModeStateOne, _parameters.ModeStateTwo, _parameters.UseStateTwo);
                species.Name = speciesNameDataAccess.GetRandomName();
                if (_parameters.ModeStateOne == ConstraintMode.None)
                {
                    AddNode(species.GetTreeRoot(1));
                }
                if (_parameters.UseStateTwo && _parameters.ModeStateTwo == ConstraintMode.None)
                {
                    AddNode(species.GetTreeRoot(2));
                }
                AddNode(species.GetTreeRoot(3));
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
            string errors = null;
            if (rngSpecies.IsValid(out errors))
            {
                rngSpecies.Generation = _generation;
                specimens.Add(rngSpecies);
            }
        }

        private IComparer<RngSpecies> GetSpeciesComparer()
        {
            switch (_parameters.CostMode)
            {
                case GeneticCostMode.FewerOperations:
                    return new SpeciesComparerNodes();
                case GeneticCostMode.Faster:
                    return new SpeciesComparerCost();
            }
            return new SpeciesComparerNodes();
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            do
            {
                ++_iteration;
                RunOneIteration(token, provider, backgroundTaskId, persistState);
            } while (_iteration < _parameters.Iterations && !token.IsCancellationRequested);
        }

        private void RunOneIteration(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            InitializeGeneration(token, provider, backgroundTaskId, persistState);
            bool converged = false;
            while (!token.IsCancellationRequested && !converged)
            {
                _generation++;
                _randomEngine.Seed(Engine.Crypto64());

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
                converged = (Best.Generation + 10) < _generation;
                OnGenerationFinished(provider);
                //save if necessary
                //report progress?
            }
            if (converged)
            {
                IGeneticSpecimenDataAccess specimenDataAccess = provider.GetService<IGeneticSpecimenDataAccess>();
                specimenDataAccess.MarkAsConverged(Best.SpecimenId);
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
            int maxTries = specimensPerGeneration * 10;
            var logger = provider.GetService<Logger>();
            while ((_specimensNextGeneration.Count < specimensPerGeneration) && (maxTries-- > 0))
            {
                try
                {
                    RngSpecies dad = SelectRandomFitSpecimen(provider);
                    RngSpecies mom = SelectRandomFitSpecimen(provider);
                    List<RngSpecies> children = Crossover(dad, mom, provider);
                    MaybeMutate(provider, children);
                    FoldConstants(children, provider);
                    string errors = null;
                    foreach (var child in children)
                    {
                        if (child.IsValid(out errors))
                        {
                            _specimensNextGeneration.Add(child);
                        }
                        else
                        {
                            LogDetails details = new LogDetails();
                            details.AddDetail("Validation Errors", errors);
                            logger.Log("Specimen failed validation!", LoggingLevel.Warning, details);
                        }
                    }
                }
                catch (Exception ex)
                {                    
                    LogDetails details = new LogDetails();
                    details.AddDetail("Exception", ex.ToString());
                    logger.Log("Error breeding specimens!", LoggingLevel.Error, details);
                }
            }
            if (_specimensNextGeneration.Count < specimensPerGeneration)
            {
                logger.Log("Didn't fill generation!", LoggingLevel.Error);
                //something 
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
                    }                    
                }
            }            
        }                

        private void ChangeANode(TreeNode treeToMutateRoot)
        {
            //change a node
            //find a random node
            //heavily favor changing a constant by flipping a bit
            //if it's a constant, change the constant, or change the constant to a state.
            //if it's a state node, change it to the other state, or change it to a constant if that doesn't invalidate the parent.
            //if it's a binary node, flip the type.


            //TODO haven't implemented flipping binary nodes yet.
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
                List<TreeNode> newNodes = new List<TreeNode>() { new ConstantNode(_randomEngine.Next64(0, long.MaxValue))};
                if (_parameters.UseStateTwo)
                {
                    newNodes.Add(new StateTwoNode());
                }
                var newNode = _randomEngine.GetRandomElement(newNodes);
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
            else if (nodeToMutate.IsBinaryNode())
            {
                var newNode = MakeNewBinaryNode(nodeToMutate.GetFirstChild(), nodeToMutate.GetSecondChild());
                treeToMutateRoot.ReplaceAllChildReferences(nodeToMutate, newNode);
            }
            
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

        /// <summary>
        /// Adds a random node at a random leaf of the given root.
        /// </summary>
        /// <param name="treeToMutateRoot"></param>
        private void AddNode(TreeNode treeToMutateRoot)
        {
            var leafNodes = treeToMutateRoot.GetDescendants().Where(x => x.IsLeafNode).ToList();
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
                    var secondNodes = new List<TreeNode>() { new StateOneNode()};
                    if (_parameters.UseStateTwo)
                    {
                        secondNodes.Add(new StateTwoNode());
                    }
                    secondNode = _randomEngine.GetRandomElement(secondNodes);
                }
            }

            secondNode.GenerationOfOrigin = _generation;
            var parentNodes = treeToMutateRoot.GetDescendants().Where(x => x.IsChild(randomLeaf)).ToList();
            TreeNode newNode = MakeNewBinaryNode(randomLeaf, secondNode);
            treeToMutateRoot.ReplaceAllChildReferences(randomLeaf, newNode);
        }

        private TreeNode MakeNewBinaryNode(TreeNode randomLeaf, TreeNode secondNode)
        {
            List<TreeNode> possibleNodes = new List<TreeNode>(16);
            if (_parameters.AllowAdditionNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new AdditionNode(randomLeaf, secondNode), new AdditionNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowSubtractionNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new SubtractNode(randomLeaf, secondNode), new SubtractNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowMultiplicationNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new MultiplicationNode(randomLeaf, secondNode), new MultiplicationNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowDivisionNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new DivideNode(randomLeaf, secondNode), new DivideNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowOrNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new OrNode(randomLeaf, secondNode), new OrNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowXorNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new XorNode(randomLeaf, secondNode), new XorNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowAndNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new AndNode(randomLeaf, secondNode), new AndNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowLeftShiftNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new LeftShiftNode(randomLeaf, secondNode), new LeftShiftNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowRightShiftNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new RightShiftNode(randomLeaf, secondNode), new RightShiftNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowRotateLeftNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new RotateLeftNode(randomLeaf, secondNode), new RotateLeftNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowRotateRightNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new RotateRightNode(randomLeaf, secondNode), new RotateRightNode(secondNode, randomLeaf)));
            }
            if (_parameters.AllowNotNodes)
            {
                possibleNodes.Add(new NotNode(randomLeaf));
            }
            if (_parameters.AllowRemainderNodes)
            {
                possibleNodes.Add(_randomEngine.PickRandomElement(new RemainderNode(randomLeaf, secondNode), new RemainderNode(secondNode, randomLeaf)));
            }
            //possibleNodes.Add(new RindjaelNode(randomLeaf));
            TreeNode result = _randomEngine.GetRandomElement(possibleNodes);
            result.GenerationOfOrigin = _generation;
            return result;                      

        }

        /// <summary>
        /// Picks a random specimen from the current generation, via tournament selection (biased towards fitter).  
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        private RngSpecies SelectRandomFitSpecimen(ServiceProvider provider)
        {
            //tournament selection
            var globalsettingsDataAccess = provider.GetService<IConfigurationDataAccess>();
            int tournamentSize = globalsettingsDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticTournamentPopulation, 8);
            double probability = globalsettingsDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticTournamentProbability, 0.9);
            
            //select tournament size specimens.
            var specimenlist = new List<RngSpecies>(_specimens);
            _randomEngine.Shuffle(specimenlist);
            specimenlist = specimenlist.Take(tournamentSize).ToList();
            specimenlist = specimenlist.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
            //now pick from the list.
            RngSpecies selectedSpecimen = SelectByTournament(specimenlist, probability);
            return selectedSpecimen;
        }

        /// <summary>
        /// Selects a random element by tournament.  Probably should cache the probabilities.
        /// </summary>
        /// <param name="specimenlist"></param>
        /// <param name="p"></param>
        /// <returns></returns>
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
        private List<RngSpecies> Crossover(RngSpecies dad, RngSpecies mom, ServiceProvider provider)
        {
            ISpeciesNameDataAccess speciesNamer = provider.GetService<ISpeciesNameDataAccess>();
            RngSpecies son = dad.DeepCopy();
            RngSpecies daughter = mom.DeepCopy();
            son.Fitness = 0;
            daughter.Fitness = 0;
            son.Generation = _generation;
            son.Name = speciesNamer.GetRandomName();
            daughter.Name = speciesNamer.GetRandomName();
            daughter.Generation = _generation;
            son.Birthday = daughter.Birthday = DateTime.Now;

            CrossoverTree(son, daughter, 3);
            if (_parameters.ModeStateOne == ConstraintMode.None)
            {
                CrossoverTree(son, daughter, 1);
            }
            if (_parameters.UseStateTwo && _parameters.ModeStateTwo == ConstraintMode.None)
            {
                CrossoverTree(son, daughter, 2);
            }
            return new List<RngSpecies>() { son, daughter };
        }

        private void CrossoverTree(RngSpecies son, RngSpecies daughter, int treeToModify)
        {
            var sonTreeRoot = son.GetTreeRoot(treeToModify);
            var daughterTreeRoot = daughter.GetTreeRoot(treeToModify);
            TreeNode sonTreeNode = PickRandomTreeNode(sonTreeRoot);
            TreeNode daughterTreeNode = PickRandomTreeNode(daughterTreeRoot);
            sonTreeRoot.ReplaceAllChildReferences(sonTreeNode, daughterTreeNode);
            daughterTreeRoot.ReplaceAllChildReferences(daughterTreeNode, sonTreeNode);
        }

        private TreeNode PickRandomTreeNode(TreeNode sonTreeRoot)
        {
            var childrenNodes = sonTreeRoot.GetDescendants();
            return _randomEngine.GetRandomElement(childrenNodes);
        }

        private int PickTreeToModify()
        {
            List<int> treeInts = new List<int>() { 3};
            if (_parameters.ModeStateOne == ConstraintMode.None)
            {
                treeInts.Add(1);
            }
            if (_parameters.UseStateTwo && _parameters.ModeStateTwo == ConstraintMode.None)
            {
                treeInts.Add(2);
            }
            return _randomEngine.GetRandomElement(treeInts);
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
                specimen.Seed = _randomEngine.Next64();
                randomTest = new RandomnessSimulation(_parameters.Level, engine, specimen.Seed);
                randomTest.Start(token, provider, backgroundTaskId, false);
                specimen.Fitness = randomTest.Iterations;
                specimen.TestsPassed = randomTest.TestsPassed;
                specimen.NameConstants();
                IGeneticSpecimenDataAccess specimenDataAccess = provider.GetService<IGeneticSpecimenDataAccess>();
                specimen.SpecimenId = specimenDataAccess.CreateSpecimen(specimen, _simulationId);                
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
                //An exception means there's something fatal.
                specimen.Fitness = -1;
                var logger = provider.GetService<Logger>();
                var details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                logger.Log("Error trying to evaluate fitness!", LoggingLevel.Information, details);
            }
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            
        }
    }
}
