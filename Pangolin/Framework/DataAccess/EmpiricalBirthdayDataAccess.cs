using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EnderPi.Framework.DataAccess
{
    public class EmpiricalBirthdayDataAccess : IEmpiricalBirthdayDataAccess
    {
        private string _connectionString;

        public EmpiricalBirthdayDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes a birthday simulation record to the birthday table.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="seed"></param>
        /// <param name="birthdaysInTheYear"></param>
        public void WriteBirthdaySimulationRecord(int simulationId, ulong seed, int birthdaysInTheYear)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateBirthdayEmpirical]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = simulationId;
                    var param = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    param.Precision = 20;
                    param.Scale = 0;
                    param.Value = seed;
                    command.Parameters.Add("@NumberOfBirthdays", SqlDbType.Int).Value = birthdaysInTheYear;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Writes a number of duplicates for a specific simulation and iteration.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="iterationNumber"></param>
        /// <param name="duplicates"></param>
        public void WriteDuplicatesForBirthdaySimulation(int simulationId, int iterationNumber, int duplicates)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateBirthdayEmpiricalDetail]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = simulationId;
                    command.Parameters.Add("@DetailId", SqlDbType.Int).Value = iterationNumber;
                    command.Parameters.Add("@NumberOfDuplicates", SqlDbType.Int).Value = duplicates;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }




    }
}
