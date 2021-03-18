using EnderPi.Framework.Pocos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EnderPi.Framework.DataAccess
{
    public class RandomnessSimulationDataAccess : IRandomnessSimulationDataAccess
    {
        private string _connectionString;

        public RandomnessSimulationDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateRandomnessSimulation(RandomnessSimulationPoco poco)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateRandomnessSimulation]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = poco.SimulationId;
                    command.Parameters.Add("@NumbersGenerated", SqlDbType.BigInt).Value = poco.NumbersGenerated;
                    command.Parameters.Add("@TargetNumbersGenerated", SqlDbType.BigInt).Value = poco.TargetNumbersGenerated;
                    command.Parameters.Add("@Result", SqlDbType.Int).Value = (int)poco.Result;
                    command.Parameters.Add("@RandomNumberEngine", SqlDbType.VarChar, 200).Value = poco.RandomNumberEngine;
                    command.Parameters.Add("@Description", SqlDbType.VarChar, 500).Value = SqlHelper.WriteNullableString(poco.Description);                    
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
