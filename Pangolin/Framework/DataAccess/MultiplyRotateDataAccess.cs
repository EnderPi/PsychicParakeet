using EnderPi.Framework.Pocos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EnderPi.Framework.DataAccess
{
    public class MultiplyRotateDataAccess : IMultiplyRotateDataAccess
    {
        private string _connectionString;

        public MultiplyRotateDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes the specimen to the database.
        /// </summary>
        /// <param name="specimen"></param>
        /// <param name="geneticSimulationId"></param>
        /// <returns></returns>
        public void InsertResult(MultiplyRotateResult result)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[InsertMultiplyRotate]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.BigInt).Value = Convert.ToInt64(result.Multiplier);
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = Convert.ToInt16(result.Rotate);
                    command.Parameters.Add("@Period", SqlDbType.BigInt).Value = Convert.ToInt64(result.Period);
                    for (int i = 0; i < 32; i++)
                    {
                        command.Parameters.Add($"@LinearityBit{i}", SqlDbType.Int).Value = result.Linearity[i];
                    }
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool RowExists(uint multiplier, int rotate)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MultiplyRotate32Exists]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.BigInt).Value = Convert.ToInt64(multiplier);
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = Convert.ToInt32(rotate);

                    sqlConnection.Open();
                    var result = command.ExecuteScalar();
                    return result != null;
                }
            }
        }
    }
}
