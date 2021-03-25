using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Data access class for the GeneticRng.GeneticSimulation Table
    /// </summary>
    public class GeneticSimulationDataAccess : IGeneticSimulationDataAccess
    {
        private string _connectionString;

        public GeneticSimulationDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a record in the GeneticRng.GeneticSimulation Table.
        /// </summary>
        /// <param name="geneticSimulation"></param>
        /// <returns>The identity of the created simulation record.</returns>
        public int CreateGeneticSimulation(GeneticParameters geneticSimulation)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[CreateGeneticSimulation]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TestLevel", SqlDbType.Int).Value = (int)geneticSimulation.Level;
                    command.Parameters.Add("@ModeStateOne", SqlDbType.Int).Value = (int)geneticSimulation.ModeStateOne;
                    command.Parameters.Add("@ModeStateTwo", SqlDbType.Int).Value = (int)geneticSimulation.ModeStateTwo;
                    command.Parameters.Add("@CostMode", SqlDbType.Int).Value = (int)geneticSimulation.CostMode;
                    command.Parameters.Add("@UseStateTwo", SqlDbType.Bit).Value = geneticSimulation.UseStateTwo;
                    command.Parameters.Add("@AllowAdditionNodes", SqlDbType.Bit).Value = geneticSimulation.AllowAdditionNodes;
                    command.Parameters.Add("@AllowSubtractionNodes", SqlDbType.Bit).Value = geneticSimulation.AllowSubtractionNodes;
                    command.Parameters.Add("@AllowMultiplicationNodes", SqlDbType.Bit).Value = geneticSimulation.AllowMultiplicationNodes;
                    command.Parameters.Add("@AllowDivisionNodes", SqlDbType.Bit).Value = geneticSimulation.AllowDivisionNodes;
                    command.Parameters.Add("@AllowRemainderNodes", SqlDbType.Bit).Value = geneticSimulation.AllowRemainderNodes;
                    command.Parameters.Add("@AllowRightShiftNodes", SqlDbType.Bit).Value = geneticSimulation.AllowRightShiftNodes;
                    command.Parameters.Add("@AllowLeftShiftNodes", SqlDbType.Bit).Value = geneticSimulation.AllowLeftShiftNodes;
                    command.Parameters.Add("@AllowRotateLeftNodes", SqlDbType.Bit).Value = geneticSimulation.AllowRotateLeftNodes;
                    command.Parameters.Add("@AllowRotateRightNodes", SqlDbType.Bit).Value = geneticSimulation.AllowRotateRightNodes;
                    command.Parameters.Add("@AllowAndNodes", SqlDbType.Bit).Value = geneticSimulation.AllowAndNodes;
                    command.Parameters.Add("@AllowOrNodes", SqlDbType.Bit).Value = geneticSimulation.AllowOrNodes;
                    command.Parameters.Add("@AllowNotNodes", SqlDbType.Bit).Value = geneticSimulation.AllowNotNodes;
                    command.Parameters.Add("@AllowXorNodes", SqlDbType.Bit).Value = geneticSimulation.AllowXorNodes;
                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets the simulation of the given Id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GeneticSimulationPoco GetSimulation(int id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[GetGeneticSimulation]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    sqlConnection.Open();
                    return ReadSimulationRecords(command).FirstOrDefault();
                }
            }
        }

        private IEnumerable<GeneticSimulationPoco> ReadSimulationRecords(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int testLevelPos = reader.GetOrdinal("TestLevel");
                int modeStateOnePos = reader.GetOrdinal("ModeStateOne");
                int modeStateTwoPos = reader.GetOrdinal("ModeStateTwo");
                int costModePos = reader.GetOrdinal("CostMode");
                int useStateTwoPos = reader.GetOrdinal("UseStateTwo");
                int allowAdditionNodesPos = reader.GetOrdinal("AllowAdditionNodes");
                int allowSubtractionNodesPos = reader.GetOrdinal("AllowSubtractionNodes");
                int allowMultiplicationNodesPos = reader.GetOrdinal("AllowMultiplicationNodes");
                int allowDivisionNodesPos = reader.GetOrdinal("AllowDivisionNodes");
                int allowRemainderNodesPos = reader.GetOrdinal("AllowRemainderNodes");
                int allowRightShiftNodesPos = reader.GetOrdinal("AllowRightShiftNodes");
                int allowLeftShiftNodesPos = reader.GetOrdinal("AllowLeftShiftNodes");
                int allowRotateLeftNodesPos = reader.GetOrdinal("AllowRotateLeftNodes");
                int allowRotateRightNodesPos = reader.GetOrdinal("AllowRotateRightNodes");
                int allowAndNodesPos = reader.GetOrdinal("AllowAndNodes");
                int allowOrNodesPos = reader.GetOrdinal("AllowOrNodes");
                int allowNotNodesPos = reader.GetOrdinal("AllowNotNodes");
                int allowXorNodesPos = reader.GetOrdinal("AllowXorNodes");
                while (reader.Read())
                {
                    GeneticSimulationPoco simulationPoco = new GeneticSimulationPoco();
                    simulationPoco.Id = reader.GetInt32(idPos);
                    simulationPoco.Level = (TestLevel)reader.GetInt32(testLevelPos);
                    simulationPoco.ModeStateOne = (ConstraintMode)reader.GetInt32(modeStateOnePos);
                    simulationPoco.ModeStateTwo = (ConstraintMode)reader.GetInt32(modeStateTwoPos);
                    simulationPoco.CostMode = (GeneticCostMode)reader.GetInt32(costModePos);
                    simulationPoco.UseStateTwo = reader.GetBoolean(useStateTwoPos);
                    simulationPoco.AllowAdditionNodes = reader.GetBoolean(allowAdditionNodesPos);
                    simulationPoco.AllowSubtractionNodes = reader.GetBoolean(allowSubtractionNodesPos);
                    simulationPoco.AllowMultiplicationNodes = reader.GetBoolean(allowMultiplicationNodesPos);
                    simulationPoco.AllowDivisionNodes = reader.GetBoolean(allowDivisionNodesPos);
                    simulationPoco.AllowRemainderNodes = reader.GetBoolean(allowRemainderNodesPos);
                    simulationPoco.AllowRightShiftNodes = reader.GetBoolean(allowRightShiftNodesPos);
                    simulationPoco.AllowLeftShiftNodes = reader.GetBoolean(allowLeftShiftNodesPos);
                    simulationPoco.AllowRotateLeftNodes = reader.GetBoolean(allowRotateLeftNodesPos);
                    simulationPoco.AllowRotateRightNodes = reader.GetBoolean(allowRotateRightNodesPos);
                    simulationPoco.AllowAndNodes = reader.GetBoolean(allowAndNodesPos);
                    simulationPoco.AllowOrNodes = reader.GetBoolean(allowOrNodesPos);
                    simulationPoco.AllowNotNodes = reader.GetBoolean(allowNotNodesPos);
                    simulationPoco.AllowXorNodes = reader.GetBoolean(allowXorNodesPos);
                    yield return simulationPoco;
                }
            }
        }

    }
}
