using System;
using System.Collections.Generic;
using EnderPi.Framework.Pocos;
using System.Data.SqlClient;
using System.Data;
using EnderPi.Framework.BackgroundWorker;
using System.Linq;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Data Access for the Simulations.Simulation table.
    /// </summary>
    public class SimulationDataAccess
    {
        /// <summary>
        /// The database connection string.
        /// </summary>
        private string _connectionString;

        public SimulationDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes the record to the database, effectively adding it to the work queue.
        /// </summary>
        /// <param name="simulation"></param>
        public void CreateSimulation(SimulationPoco simulation)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateSimulation]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SaveFile", SqlDbType.VarChar, 200).Value = SqlHelper.WriteNullableString(simulation.SaveFile);
                    command.Parameters.Add("@Description", SqlDbType.VarChar, 200).Value = SqlHelper.WriteNullableString(simulation.Description);
                    command.Parameters.Add("@LastTimeSaved", SqlDbType.DateTime).Value = SqlHelper.WriteNullableDateTime(simulation.TimeLastSaved);
                    command.Parameters.Add("@SimulationObject", SqlDbType.VarChar, -1).Value = simulation.SimulationObject;
                    command.Parameters.Add("@IsRunning", SqlDbType.Bit).Value = simulation.IsRunning;
                    command.Parameters.Add("@IsFinished", SqlDbType.Bit).Value = simulation.IsFinished;
                    command.Parameters.Add("@PercentComplete", SqlDbType.Float).Value = simulation.PercentComplete;
                    command.Parameters.Add("@Priority", SqlDbType.Int).Value = simulation.Priority;
                    command.Parameters.Add("@IsCancelled", SqlDbType.Bit).Value = simulation.IsCancelled;
                    command.Parameters.Add("@TimeStarted", SqlDbType.DateTime).Value = SqlHelper.WriteNullableDateTime(simulation.TimeStarted);
                    command.Parameters.Add("@PercentCompleteWhenStarted", SqlDbType.Float).Value = simulation.PercentCompleteWhenStarted;
                    command.Parameters.Add("@EstimatedFinishTime", SqlDbType.DateTime).Value = SqlHelper.WriteNullableDateTime(simulation.EstimatedFinishTime);
                    command.Parameters.Add("@TimeCompleted", SqlDbType.DateTime).Value = SqlHelper.WriteNullableDateTime(simulation.TimeCompleted);
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the next ready task that is not in flight.  May or may not have a save file.
        /// </summary>
        /// <param name="currentlyRunningSimulations"></param>
        /// <returns></returns>
        internal SimulationPoco GetNextReadyTaskNotInFlight(List<RunningSimulationReference> currentlyRunningSimulations)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[GetNextReadyTaskNotInFlight]", sqlConnection))
                {
                    SqlParameter parameter;
                    parameter = command.Parameters.AddWithValue("@CurrentlyRunningSimulations", CreateDataTable(currentlyRunningSimulations));
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[Simulations].[SimulationIds]";
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadSimulationPoco(command).FirstOrDefault();
                }
            }            
        }

        internal void MarkSimulationComplete(int simulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MarkSimulationComplete]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = simulationId;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        internal SimulationPoco GetTask(int simulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[GetSimulationById]", sqlConnection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = simulationId;
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadSimulationPoco(command).FirstOrDefault();
                }
            }
        }

        private static DataTable CreateDataTable(List<RunningSimulationReference> currentlyRunningSimulations)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));            
            if (currentlyRunningSimulations != null)
            {
                foreach (var simulation in currentlyRunningSimulations)
                {
                    table.Rows.Add(simulation.SimulationId);
                }
            }
            return table;
        }

        internal SimulationPoco GetNextTaskThatHasntBeenStarted(List<RunningSimulationReference> currentlyRunningSimulations)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[GetNextTaskNotInFlightWithNoFileSave]", sqlConnection))
                {
                    SqlParameter parameter;
                    parameter = command.Parameters.AddWithValue("@CurrentlyRunningSimulations", CreateDataTable(currentlyRunningSimulations));
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[Simulations].[SimulationIds]";
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadSimulationPoco(command).FirstOrDefault();
                }
            }
        }

        internal void MarkSimulationAsCanceled(int simulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MarkSimulationCanceled]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = simulationId;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<SimulationPoco> ReadSimulationPoco(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int saveFilePos = reader.GetOrdinal("SaveFile");
                int descriptionPos = reader.GetOrdinal("Description");
                int timeLastSavedPos = reader.GetOrdinal("LastTimeSaved");
                int simulationObjectPos = reader.GetOrdinal("SimulationObject");
                int isRunningPos = reader.GetOrdinal("IsRunning");
                int isFinishedPos = reader.GetOrdinal("IsFinished");
                int percentCompletePos = reader.GetOrdinal("PercentComplete");
                int priorityPos = reader.GetOrdinal("Priority");
                int isCancelledPos = reader.GetOrdinal("IsCancelled");
                int timeStartedPos = reader.GetOrdinal("TimeStarted");
                int percentCompleteWhenStartedPos = reader.GetOrdinal("PercentCompleteWhenStarted");
                int estimatedFinishTimePos = reader.GetOrdinal("EstimatedFinishTime");
                int timeCompletedPos = reader.GetOrdinal("TimeCompleted");
                while (reader.Read())
                {
                    SimulationPoco simulationPoco = new SimulationPoco();
                    simulationPoco.Id = reader.GetInt32(idPos);
                    simulationPoco.SaveFile = SqlHelper.ReadNullableString(reader, saveFilePos);
                    simulationPoco.Description = SqlHelper.ReadNullableString(reader, descriptionPos);
                    simulationPoco.TimeLastSaved = SqlHelper.ReadNullableDateTime(reader, timeLastSavedPos);
                    simulationPoco.SimulationObject = SqlHelper.ReadNullableString(reader, simulationObjectPos);
                    simulationPoco.IsRunning = reader.GetBoolean(isRunningPos);
                    simulationPoco.IsFinished = reader.GetBoolean(isFinishedPos);
                    simulationPoco.PercentComplete = reader.GetDouble(percentCompletePos);
                    simulationPoco.Priority = reader.GetInt32(priorityPos);
                    simulationPoco.IsCancelled = reader.GetBoolean(isCancelledPos);
                    simulationPoco.TimeStarted = SqlHelper.ReadNullableDateTime(reader, timeStartedPos);
                    simulationPoco.PercentCompleteWhenStarted = reader.GetDouble(percentCompleteWhenStartedPos);
                    simulationPoco.EstimatedFinishTime = SqlHelper.ReadNullableDateTime(reader, estimatedFinishTimePos);
                    simulationPoco.TimeCompleted = SqlHelper.ReadNullableDateTime(reader, timeCompletedPos);
                    yield return simulationPoco;
                }
            }
        }

        internal void UpdateFileName(int simulationId, string fullFileName)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[UpdateSimulationFileName]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = simulationId;
                    command.Parameters.Add("@FileName", SqlDbType.VarChar, 200).Value = fullFileName;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void UpdateProgress(int backgroundTaskId, double percentComplete, DateTime? estimatedFinishTime)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[UpdateSimulationProgress]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = backgroundTaskId;
                    command.Parameters.Add("@PercentComplete", SqlDbType.Float).Value = percentComplete;
                    command.Parameters.Add("@EstimatedFinishTime", SqlDbType.DateTime).Value = SqlHelper.WriteNullableDateTime(estimatedFinishTime);
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
