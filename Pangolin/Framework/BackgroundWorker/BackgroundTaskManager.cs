using EnderPi.Framework.Messaging;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Simulation;
using System.Xml;
using System.Collections.Generic;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Caching;

namespace EnderPi.Framework.BackgroundWorker
{
    /// <summary>
    /// Manages background task logic.
    /// </summary>
    public class BackgroundTaskManager : IBackgroundTaskManager
    {
        private DataContractSerializer _serializer;

        private SimulationDataAccess _simulationDataAccess;

        private Logger _logger;

        private IConfigurationDataAccess _configurationDataAccess;

        private TimedCache _savingCache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataAccess">Data access object for the simulation table.</param>
        public BackgroundTaskManager(SimulationDataAccess dataAccess, Logger logger, IConfigurationDataAccess configurationDataAccess)
        {
            _simulationDataAccess = dataAccess;
            _serializer = new DataContractSerializer(typeof(LongRunningTask), GetKnownTypes());
            _logger = logger;
            _configurationDataAccess = configurationDataAccess;
            _savingCache = new TimedCache();
        }

        /// <summary>
        /// The list of known LongRunningTask types.  When you create a new LongRunningTask, you should add it here.
        /// </summary>
        /// <returns></returns>
        private Type[] GetKnownTypes()
        {
            return new Type[] { typeof(GetBirthdayValuesSimulation), typeof(RandomnessSimulation), typeof(TestSimulation) };
        }

        /// <summary>
        /// Submit the given task for processing in the background.  Probably mostly called from the UI, where a user submits simulation jobs.
        /// </summary>
        /// <param name="task">The task to submit</param>
        public void SubmitBackgroundTask(LongRunningTask task)
        {
            string serializedTask = Serialize(task);
            SimulationPoco simulation = new SimulationPoco() { SimulationObject = serializedTask };
            _simulationDataAccess.CreateSimulation(simulation);
        }

        /// <summary>
        /// Serializes a task to a string using the data contract serializer.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Serialize(LongRunningTask task)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    _serializer.WriteObject(xmlWriter, task);
                }
                return stringWriter.GetStringBuilder().ToString();
            }
        }


        /// <summary>
        /// Deserializes a string into an event message.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public LongRunningTask DeserializeTask(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var xmlreader = XmlReader.Create(stringReader))
                {
                    return (LongRunningTask)_serializer.ReadObject(xmlreader);
                }
            }
        }

        public void MarkSimulationComplete(int simulationId)
        {
            _simulationDataAccess.MarkSimulationComplete(simulationId);
        }

        public SimulationPoco GetTask(int simulationId)
        {
            return _simulationDataAccess.GetTask(simulationId);
        }

        /// <summary>
        /// Gets the next task from the table that is ready to run.  (used by the runtime)
        /// </summary>
        /// <remarks>
        /// Tries to get a task that was aborted and has a savefile, otherwise gets a task that hasn't been started.
        /// </remarks>
        /// <returns></returns>
        public RunningSimulationReference GetNextTask(List<RunningSimulationReference> currentlyRunningSimulations)
        {
            RunningSimulationReference nextTask = null;
            var nextReadyTaskNotInFlight = _simulationDataAccess.GetNextReadyTaskNotInFlight(currentlyRunningSimulations);
            if (nextReadyTaskNotInFlight != null)
            {
                nextTask = new RunningSimulationReference();
                nextTask.SimulationId = nextReadyTaskNotInFlight.Id;
                if (nextReadyTaskNotInFlight.SaveFile != null)
                {
                    LongRunningTask task = ReadTaskFromFile(nextReadyTaskNotInFlight.SaveFile);
                    if (task != null)
                    {
                        nextTask.Task = task;
                    }
                }
                //If it didn't have a savefile or the save file failed to load, just start over from the initial state.
                if (nextTask.Task == null)
                {
                    nextTask.Task = DeserializeTask(nextReadyTaskNotInFlight.SimulationObject);
                }
            }
            return nextTask;
        }

        private LongRunningTask ReadTaskFromFile(string saveFile)
        {
            LongRunningTask task = null;
            if (File.Exists(saveFile))
            {
                FileStream fs = new FileStream(saveFile, FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    task = (LongRunningTask)formatter.Deserialize(fs);
                }
                catch (SerializationException ex)
                {
                    LogDetails details = new LogDetails();
                    details.AddDetail("Exception", ex.ToString());
                    details.AddDetail("File Name", saveFile);
                    _logger.Log("Failed to read long running task from file", LoggingLevel.Error, details);
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                _logger.Log($"Failed to find filename {saveFile} to load task from, falling back to initial state.", LoggingLevel.Warning);
            }
            return task;
        }

        public void MarkSimulationAsCanceled(int simulationId)
        {
            _simulationDataAccess.MarkSimulationAsCanceled(simulationId);
        }

        /// <summary>
        /// Saves the simulation if it hasn't been saved recently.  Safe to call pretty often, caches relevant data so it only hits the database every 15 minutes.
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="backgroundTaskId"></param>
        public void SaveIfNecessary(LongRunningTask simulation, int backgroundTaskId)
        {
            string cacheKey = $"IsItTimeToSave_{backgroundTaskId}";
            if (_savingCache.Fetch(cacheKey, () => IsItTimeToSaveSimulation(backgroundTaskId)))
            {
                //Really don't want to cache a TRUE, because it will save again.
                _savingCache.DropCacheItem(cacheKey);
                SaveTask(simulation, backgroundTaskId);
            }
        }

        private bool IsItTimeToSaveSimulation(int backgroundTaskId)
        {
            DateTime now = DateTime.Now;
            var simulation = _simulationDataAccess.GetTask(backgroundTaskId);
            TimeSpan timeSinceLastSave;
            if (simulation.TimeLastSaved.HasValue)
            {
                timeSinceLastSave = now - simulation.TimeLastSaved.Value;
            }
            else
            {
                timeSinceLastSave = now - simulation.TimeStarted.Value;
            }
            var minutesBetweenFileSaves = _configurationDataAccess.GetGlobalSettingInt(GlobalSettings.MinutesBetweenFileSaves, 240);
            return timeSinceLastSave.TotalMinutes > minutesBetweenFileSaves;
        }

        public void SaveTask(LongRunningTask simulation, int backgroundTaskId)
        {
            //TODO this doesn't do a great job of maintaining safety in the try block with all the tasks.
            try
            {
                var saveFiledirectory = _configurationDataAccess.GetGlobalSettingString(GlobalSettings.SaveFileDirectory);
                if (!string.IsNullOrWhiteSpace(saveFiledirectory))
                {
                    string fullFileName = Path.Combine(saveFiledirectory, $"Simulation_{backgroundTaskId}_{Guid.NewGuid()}.sim");
                    FileStream fs = new FileStream(fullFileName, FileMode.CreateNew);
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(fs, simulation);
                        var oldTask = GetTask(backgroundTaskId);
                        Threading.Threading.ExecuteWithoutThrowing(() => File.Delete(oldTask.SaveFile), _logger);
                        _simulationDataAccess.UpdateFileName(backgroundTaskId, fullFileName);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
                else
                {
                    _logger.Log("Missing directory configuration for saving task!", LoggingLevel.Error);
                }
            }
            catch (Exception ex)
            {
                LogDetails details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                details.AddDetail("Id", backgroundTaskId.ToString());
                //It's definitely an error if we can't save a file, but I don't want to crash the simulation.  Might just need to clean out files.
                _logger.Log($"Failed to save file for {backgroundTaskId}", LoggingLevel.Error);
            }
        }

        /// <summary>
        /// Reports percent complete by updating the record in the simulation table.  Estimates completion time as well.
        /// </summary>
        /// <param name="backgroundTaskId"></param>
        /// <param name="v"></param>
        public void ReportProgress(int backgroundTaskId, double percentComplete)
        {
            //estimate finish time.....
            DateTime now = DateTime.Now;
            DateTime? estimatedFinishTime = null;
            var simulation = GetTask(backgroundTaskId);
            var percentFinishSinceStarted = percentComplete - simulation.PercentCompleteWhenStarted;
            var millisecondsSinceStart = (now - simulation.TimeStarted.Value).TotalMilliseconds;
            var velocity = percentFinishSinceStarted / millisecondsSinceStart;
            if (velocity != 0)
            {
                var timeLeft = (100.0 - percentComplete) / velocity;
                estimatedFinishTime = now.AddMilliseconds(timeLeft);
            }
            _simulationDataAccess.UpdateProgress(backgroundTaskId, percentComplete, estimatedFinishTime);
        }
    }
}
