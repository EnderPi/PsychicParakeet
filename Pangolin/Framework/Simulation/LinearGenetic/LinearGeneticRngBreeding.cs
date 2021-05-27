using EnderPi.Framework.BackgroundWorker;
using System;
using System.Collections.Generic;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Extensions;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using System.Runtime.Serialization;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Simulation.Genetic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    public class LinearGeneticRngBreeding : LongRunningTask
    {
        /// <summary>
        /// The specimens in the current generation.
        /// </summary>
        [DataMember]
        private List<LinearGeneticSpecimen> _specimens;
        /// <summary>
        /// The specimens in the next generation.  Maybe this doesn't need to be state.
        /// TODO consider if this needs to be state.
        /// </summary>
        [DataMember]
        private List<LinearGeneticSpecimen> _specimensNextGeneration;
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
        public LinearGeneticSpecimen Biggest { get { return _specimens.OrderByDescending(x => x.ProgramLength).FirstOrDefault(); } }

        public LinearGeneticSpecimen Best { get { return _specimens.OrderByDescending(x => x.Fitness).FirstOrDefault(); } }

        private int _iteration;

        public delegate void GeneticEventHandler(object sender, LinearGeneticEventArgs e);

        public event GeneticEventHandler GenerationFinished;

        private void OnGenerationFinished(ServiceProvider provider)
        {
            LinearGeneticEventArgs e = new LinearGeneticEventArgs();
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
                logger.Log($"Error {ex}", LoggingLevel.Error);
            }

            Threading.Threading.ExecuteWithoutThrowing(() => GenerationFinished?.Invoke(this, e), logger);
        }

        /// <summary>
        /// Basic constructor.  No real initial state.
        /// </summary>        
        public LinearGeneticRngBreeding(GeneticParameters parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Constructor if you want to pass in some starting species.
        /// </summary>
        /// <param name="initialSpecies"></param>
        public LinearGeneticRngBreeding(List<LinearGeneticSpecimen> initialSpecies)
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
            int speciesPerGeneration = configDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticGenerationSize, 512);
            _specimens = new List<LinearGeneticSpecimen>(speciesPerGeneration);
            int initialSize = configDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticLinearProgramInitialSize, 4);
            while (_specimens.Count < speciesPerGeneration)
            {
                var species = new LinearGeneticSpecimen();
                species.Name = speciesNameDataAccess.GetRandomName();                
                RandomizeProgram(species, initialSize);
                AddSpecies(_specimens, species);
            }
            foreach (var specimen in _specimens)
            {
                EvaluateFitness(specimen, token, provider, backgroundTaskId);
                if (token.IsCancellationRequested) break;
            }
            _specimens = _specimens.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
            OnGenerationFinished(provider);
        }

        /// <summary>
        /// Randomizes the program by adding 32 random commands, then copies a random register to the output (ensuring validity).  
        /// Doesn't touch seed program since that isn't very important.
        /// </summary>
        /// <param name="species"></param>
        private void RandomizeProgram(LinearGeneticSpecimen species, int initialSize)
        {                     
            List<Command8099> program = new List<Command8099>();
            for (int i=0; i< initialSize; i++)
            {
                program.Add(GetRandomCommand());
            }
            int randomStateRegister = _randomEngine.NextInt(0, 2);
            program.Add(new XorRegister(7, randomStateRegister));
            species.GenerationProgram = Machine8099ProgramCleaner.CleanProgram(program.ToArray());
            species.SeedProgram = new List<Command8099>();
        }

        /// <summary>
        /// Creates a random command with a random target and constant, obeying the parameters.
        /// </summary>
        /// <returns></returns>
        private Command8099 GetRandomCommand()
        {
            List<Command8099> potentialCommmands = new List<Command8099>();            
            int targetRegister = _randomEngine.NextInt(0, 3);
            int sourceRegister = _randomEngine.NextInt(0, 3);
            if (targetRegister == 3) targetRegister = 7;
            if (sourceRegister == 3) sourceRegister = 7;
            ulong randomUlong = _randomEngine.Next64();
            int randomInt = _randomEngine.NextInt(1, 63);

            potentialCommmands.Add(new MoveRegister(targetRegister, sourceRegister));
            potentialCommmands.Add(new MoveConstant(targetRegister, randomUlong));
            potentialCommmands.Add(new IntronCommand());
            if (_parameters.AllowAdditionNodes)
            {
                potentialCommmands.Add(new AddRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new AddConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowSubtractionNodes)
            {
                potentialCommmands.Add(new SubtractRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new SubtractConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowMultiplicationNodes)
            {
                potentialCommmands.Add(new MultiplyRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new MultiplyConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowDivisionNodes)
            {
                potentialCommmands.Add(new DivideRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new DivideConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowRemainderNodes)
            {
                potentialCommmands.Add(new RemainderRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new RemainderConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowAndNodes)
            {
                potentialCommmands.Add(new AndRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new AndConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowOrNodes)
            {
                potentialCommmands.Add(new OrRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new OrConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowXorNodes)
            {
                potentialCommmands.Add(new XorRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new XorConstant(targetRegister, randomUlong));
            }
            if (_parameters.AllowNotNodes)
            {
                potentialCommmands.Add(new Not(targetRegister));                
            }
            if (_parameters.AllowRightShiftNodes)
            {
                potentialCommmands.Add(new RightShiftRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new RightShiftConstant(targetRegister, randomInt));
            }
            if (_parameters.AllowLeftShiftNodes)
            {
                potentialCommmands.Add(new LeftShiftRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new LeftShiftConstant(targetRegister, randomInt));
            }
            if (_parameters.AllowRotateRightNodes)
            {
                potentialCommmands.Add(new RotateRightRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new RotateRightConstant(targetRegister, randomInt));
            }
            if (_parameters.AllowRotateLeftNodes)
            {
                potentialCommmands.Add(new RotateLeftRegister(targetRegister, sourceRegister));
                potentialCommmands.Add(new RotateLeftConstant(targetRegister, randomInt));
            }
            if (_parameters.AllowRindjael)
            {
                potentialCommmands.Add(new Rindjael(targetRegister));
            }
            return _randomEngine.GetRandomElement(potentialCommmands);
        }


        /// <summary>
        /// Adds a species to the list, persists it to the underlying table.
        /// </summary>
        /// <param name="specimens"></param>
        /// <param name="LinearGeneticSpecimen"></param>
        private void AddSpecies(List<LinearGeneticSpecimen> specimens, LinearGeneticSpecimen LinearGeneticSpecimen)
        {
            string errors = null;
            if (LinearGeneticSpecimen.IsValid(out errors))
            {
                LinearGeneticSpecimen.Generation = _generation;
                specimens.Add(LinearGeneticSpecimen);
            }
        }

        private IComparer<LinearGeneticSpecimen> GetSpeciesComparer()
        {
            switch (_parameters.CostMode)
            {
                case GeneticCostMode.FewerOperations:
                    return new LinearSpeciesComparer();
                case GeneticCostMode.Faster:
                    return new LinearSpeciesComparer();
            }
            return new LinearSpeciesComparer();
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
                IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
                _generation++;
                _randomEngine.Seed(Engine.Crypto64());

                _specimensNextGeneration = new List<LinearGeneticSpecimen>();
                _specimensNextGeneration.AddRange(EliteSpecimens(provider));
                SelectAndBreed(provider);
                _specimens = _specimensNextGeneration;  //replacing....

                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticMaxParallelism, 4);
                options.CancellationToken = token;
                Parallel.ForEach(_specimens, options, x => EvaluateFitness(x, token, provider, backgroundTaskId));
                
                _specimens = _specimens.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
                double averageFitness = _specimens.Average(x => x.Fitness);
                double medianFitness = _specimens[_specimens.Count / 2].Fitness;
                int convergedConstant = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticConvergedAgeConstant, 8);
                converged = (Best.Generation + convergedConstant) < _generation;
                OnGenerationFinished(provider);
                //save if necessary
                //report progress?
            }
            if (converged)
            {
                var specimenDataAccess = provider.GetService<ILinearGeneticDataAccess>();
                specimenDataAccess.MarkAsConverged(Best.SpecimenId);
            }
        }




        private List<LinearGeneticSpecimen> EliteSpecimens(ServiceProvider provider)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            int specimensToTake = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticEliteSize, 1);
            return _specimens.Take(specimensToTake).ToList();
        }

        private void SelectAndBreed(ServiceProvider provider)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            int specimensPerGeneration = configurationDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticGenerationSize, 512);
            int maxTries = specimensPerGeneration * 10;
            var logger = provider.GetService<Logger>();
            while ((_specimensNextGeneration.Count < specimensPerGeneration) && (maxTries-- > 0))
            {
                try
                {
                    LinearGeneticSpecimen dad = SelectRandomFitSpecimen(provider);
                    LinearGeneticSpecimen mom = SelectRandomFitSpecimen(provider);
                    List<LinearGeneticSpecimen> children = Crossover(dad, mom, provider);
                    MaybeMutate(provider, children);                    
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
                            logger.Log("Specimen failed validation!", LoggingLevel.Debug, details);
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
            }
        }
        
        private void MaybeMutate(ServiceProvider provider, List<LinearGeneticSpecimen> children)
        {
            IConfigurationDataAccess configurationDataAccess = provider.GetService<IConfigurationDataAccess>();
            double mutationProbability = configurationDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticMutationChance, (double)1.0 / 16);            

            foreach (var child in children)
            {                

                while (_randomEngine.NextDoubleInclusive() < mutationProbability)
                {
                    int programLength = child.GenerationProgram.Count;
                    int randomIndex = _randomEngine.NextInt(0, programLength - 1);
                    var newCommand = GetRandomCommand();
                    uint choice = _randomEngine.Next32(1, 5);                    
                    switch (choice)
                    {
                        case 1:
                            //insert a random command at a random point                            
                            child.GenerationProgram.Insert(randomIndex, newCommand);                            
                            break;

                        case 2:
                            //remove a random command                            
                            child.GenerationProgram.RemoveAt(randomIndex);
                            break;

                        case 3:
                            //change a random command
                            child.GenerationProgram[randomIndex] = newCommand;
                            break;
                        case 4:
                            if (child.GenerationProgram[randomIndex] is BinaryRegisterConstantCommand constantCommand)
                            {
                                constantCommand.Constant = _randomEngine.Next64();
                            }
                            break;
                        case 5:
                            if (child.GenerationProgram[randomIndex] is BinaryRegisterIntCommand intCommand)
                            {
                                intCommand.Constant = _randomEngine.NextInt(1, 63);
                            }
                            break;
                    }
                }
                child.GenerationProgram = Machine8099ProgramCleaner.CleanProgram(child.GenerationProgram.ToArray());
            }
        }
                                
        
        /// <summary>
        /// Picks a random specimen from the current generation, via tournament selection (biased towards fitter).  
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        private LinearGeneticSpecimen SelectRandomFitSpecimen(ServiceProvider provider)
        {
            //tournament selection
            var globalsettingsDataAccess = provider.GetService<IConfigurationDataAccess>();
            int tournamentSize = globalsettingsDataAccess.GetGlobalSettingInt(GlobalSettings.GeneticTournamentPopulation, 8);
            double probability = globalsettingsDataAccess.GetGlobalSettingDouble(GlobalSettings.GeneticTournamentProbability, 0.9);

            //select tournament size specimens.
            var specimenlist = new List<LinearGeneticSpecimen>(_specimens);
            _randomEngine.Shuffle(specimenlist);
            specimenlist = specimenlist.Take(tournamentSize).ToList();
            specimenlist = specimenlist.OrderByDescending(x => x, GetSpeciesComparer()).ToList();
            //now pick from the list.
            LinearGeneticSpecimen selectedSpecimen = SelectByTournament(specimenlist, probability);
            return selectedSpecimen;
        }

        /// <summary>
        /// Selects a random element by tournament.  Probably should cache the probabilities.
        /// </summary>
        /// <param name="specimenlist"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private LinearGeneticSpecimen SelectByTournament(List<LinearGeneticSpecimen> specimenlist, double p)
        {
            double[] probs = new double[specimenlist.Count];
            double[] cumulativeprobs = new double[specimenlist.Count];
            double totalProb = 0;
            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = p * Math.Pow(1 - p, i);
                totalProb += probs[i];
            }
            probs[0] += 1 - totalProb;
            cumulativeprobs[0] = probs[0];
            for (int i = 1; i < cumulativeprobs.Length; i++)
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
        private List<LinearGeneticSpecimen> Crossover(LinearGeneticSpecimen dad, LinearGeneticSpecimen mom, ServiceProvider provider)
        {
            ISpeciesNameDataAccess speciesNamer = provider.GetService<ISpeciesNameDataAccess>();
            LinearGeneticSpecimen son = dad.DeepCopy();
            LinearGeneticSpecimen daughter = mom.DeepCopy();
            son.Fitness = 0;
            daughter.Fitness = 0;
            son.Generation = _generation;
            son.Name = speciesNamer.GetRandomName();
            daughter.Name = speciesNamer.GetRandomName();
            daughter.Generation = _generation;
            son.Birthday = daughter.Birthday = DateTime.Now;

            CrossoverTree(son, daughter);
            
            return new List<LinearGeneticSpecimen>() { son, daughter };
        }

        private void CrossoverTree(LinearGeneticSpecimen son, LinearGeneticSpecimen daughter)
        {
            int sonCrossover = _randomEngine.NextInt(0, son.GenerationProgram.Count - 1);
            int daughterCrossover = _randomEngine.NextInt(0, daughter.GenerationProgram.Count - 1);
            son.GenerationProgram = Machine8099ProgramCleaner.CleanProgram(Crossover(son.GenerationProgram, daughter.GenerationProgram, sonCrossover, daughterCrossover).ToArray());
            daughter.GenerationProgram = Machine8099ProgramCleaner.CleanProgram(Crossover(daughter.GenerationProgram, son.GenerationProgram, daughterCrossover, sonCrossover).ToArray());            
        }

        private List<Command8099> Crossover(List<Command8099> generationProgramSon, List<Command8099> generationProgramDaughter, int sonCrossover, int daughterCrossover)
        {
            var genome = generationProgramSon.Take(sonCrossover).ToList();
            for (int i=daughterCrossover; i < generationProgramDaughter.Count; i++)
            {
                genome.Add(generationProgramDaughter[i]);
            }
            return genome;
        }

        private void EvaluateFitness(LinearGeneticSpecimen specimen, CancellationToken token, ServiceProvider provider, int backgroundTaskId)
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
                var specimenDataAccess = provider.GetService<ILinearGeneticDataAccess>();
                specimen.SpecimenId = specimenDataAccess.CreateSpecimen(specimen, _simulationId);
                //todo write this record in a finally
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
