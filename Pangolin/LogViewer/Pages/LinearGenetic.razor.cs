using EnderPi.Framework.Simulation.Genetic;
using GeneticWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using EnderPi.Framework.Services;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Extensions;
using EnderPi.Framework.Simulation.LinearGenetic;

namespace GeneticWeb.Pages
{
    /// <summary>
    /// Code behind for the genetics page.
    /// </summary>
    public partial class LinearGenetic
    {
        /// <summary>
        /// Median fitness of the current generation.
        /// </summary>
        private string _medianFitness;

        private GeneticParameters _model;

        /// <summary>
        /// The current best species.
        /// </summary>
        private LinearGeneticSpecimen _bestSpecies;

        /// <summary>
        /// Current random species being displayed.
        /// </summary>
        private LinearGeneticSpecimen _randomSpecies;

        private bool _running = false;

        private string _currentGeneration;

        private string _currentIteration;

        private CancellationTokenSource _source;

        private System.Timers.Timer _timer;

        private List<List<LinearGeneticSpecimen>> _allSpecimens;

        private object _padlock;

        private EnderPi.Framework.Random.Sha256 _randomEngine;

        protected override void OnInitialized()
        {
            _padlock = new object();
            _randomEngine = new EnderPi.Framework.Random.Sha256();
            _model = new GeneticParameters();
            _model.Level = EnderPi.Framework.Simulation.RandomnessTest.TestLevel.One;
            AssignSpecies(new LinearGeneticSpecimen());
            _timer = new System.Timers.Timer(8 * 1000);
            _timer.Elapsed += HandleTimerEvent;
            _timer.AutoReset = true;
            _model.AllowAdditionNodes = true;
            _model.AllowSubtractionNodes = true;
            _model.AllowMultiplicationNodes = true;
            _model.AllowDivisionNodes = true;
            _model.AllowRemainderNodes = true;
            _model.AllowAndNodes = true;
            _model.AllowNotNodes = true;
            _model.AllowOrNodes = true;
            _model.AllowXorNodes = true;
            _model.AllowLeftShiftNodes = true;
            _model.AllowRightShiftNodes = true;
            _model.AllowRotateLeftNodes = true;
            _model.AllowRotateRightNodes = true;
            _model.AllowRindjael = false;
            _model.Iterations = 1;
        }

        public LinearGenetic()
        {

        }

        private void HandleTimerEvent(object source, ElapsedEventArgs e)
        {
            bool lockwasTaken = false;
            try
            {
                Monitor.TryEnter(_padlock, 10, ref lockwasTaken);
                if (lockwasTaken)
                {
                    if (_allSpecimens != null && _allSpecimens.Count > 0)
                    {
                        var which = _randomEngine.NextInt(0, _allSpecimens.Count - 1);
                        var nonZeroElements = _allSpecimens[which].Where(x => x.Fitness > 0).ToList();
                        int whichElement = _randomEngine.NextInt(0, nonZeroElements.Count - 1);
                        _randomSpecies = nonZeroElements[whichElement];
                        var bitmap = _randomSpecies.GetImageBitMap();
                        MemoryStream ms = new MemoryStream();
                        bitmap.Save(ms, ImageFormat.Gif);
                        var directory = configurationDataAccess.GetGlobalSettingString(EnderPi.Framework.Pocos.GlobalSettings.GeneticGifSaveDirectory);
                        if (!string.IsNullOrWhiteSpace(directory))
                        {
                            string fileName = directory + "\\" + _randomSpecies.Name + ".gif";
                            if (!File.Exists(fileName))
                            {
                                File.WriteAllBytes(fileName, ms.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogDetails details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                logger.Log("Error processing background timer!", LoggingLevel.Error, details);
            }
            finally
            {
                if (lockwasTaken)
                {
                    Monitor.Exit(_padlock);
                    InvokeAsync(() => StateHasChanged());
                }
            }

        }
        /// <summary>
        /// Starts the simulation
        ///
        ///</summary>
        public void HandleStartClick()
        {
            _allSpecimens = new List<List<LinearGeneticSpecimen>>();
            AssignSpecies(null);
            _randomSpecies = null;
            _currentGeneration = null;
            _medianFitness = null;
            _running = true;
            Thread backgroundThread = new Thread(BackgroundTaskDelegate, 10 * 1024 * 1024);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
            _timer.Enabled = true;

        }

        public void BackgroundTaskDelegate()
        {
            if (!_model.UseStateTwo)
            {
                _model.ModeStateTwo = ConstraintMode.None;
            }
            var parameters = _model.DeepCopy();
            LinearGeneticRngBreeding geneticTask = null;
            try
            {
                _source = new CancellationTokenSource();
                var token = _source.Token;
                var provider = new ServiceProvider();
                provider.RegisterService(configurationDataAccess);
                provider.RegisterService(backgroundTaskManager);
                provider.RegisterService(logger);
                provider.RegisterService(speciesNameManager);
                provider.RegisterService(geneticSimulationManager);
                provider.RegisterService(geneticSpecimenManager);
                geneticTask = new LinearGeneticRngBreeding(parameters);
                geneticTask.GenerationFinished += UpdateGeneration;
                geneticTask.Start(token, provider, 0, false);
                AssignSpecies(geneticTask.Best);
            }
            catch (Exception ex)
            {
                var details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                logger.Log("Error Running Simulation!", LoggingLevel.Error, details);
            }
            finally
            {
                _source.Dispose();
                _running = false;
                InvokeAsync(() => StateHasChanged());
                if (geneticTask != null)
                {
                    geneticTask.GenerationFinished -= UpdateGeneration;
                }
                _timer.Enabled = false;

            }
        }

        public void AssignSpecies(LinearGeneticSpecimen species)
        {
            _bestSpecies = species;
        }

        private void UpdateGeneration(object sender, LinearGeneticEventArgs e)
        {
            try
            {
                if (e != null && e.ThisGeneration != null)
                {
                    lock (_padlock)
                    {
                        _allSpecimens.Add(e.ThisGeneration);
                        var bestSpecimen = e.ThisGeneration[0];
                        AssignSpecies(bestSpecimen);
                        _currentGeneration = e.Generation.ToString("N0");
                        _medianFitness = e.ThisGeneration[e.ThisGeneration.Count / 2].Fitness.ToString("N0");
                        _currentIteration = e.Iteration.ToString();
                    }
                    InvokeAsync(() => StateHasChanged());
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString(), LoggingLevel.Error);
            }
        }

        /// <summary>
        /// Stops the simulation
        ///</summary>
        public void HandleStopClick()
        {
            Threading.ExecuteWithoutThrowing(() => _source.Cancel());
        }
    }
}
