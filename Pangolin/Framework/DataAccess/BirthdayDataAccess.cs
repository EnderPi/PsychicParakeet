using EnderPi.Framework.Pocos;
using System.Data.SqlClient;
using System.Data;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Class for data access to the birthday tests.
    /// </summary>
    public class BirthdayDataAccess : IBirthdayDataAccess
    {
        private string _connectionString;

        public BirthdayDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateBirthdayTest(BirthdayTestPoco poco)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateBirthdayTest]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = poco.SimulationId;
                    command.Parameters.Add("@NumberOfIterations", SqlDbType.Int).Value = poco.NumberOfIterations;
                    command.Parameters.Add("@Lambda", SqlDbType.Int).Value = poco.Lambda;
                    command.Parameters.Add("@PValue", SqlDbType.Float).Value = poco.PValue;
                    command.Parameters.Add("@TestResult", SqlDbType.Int).Value = (int)poco.Result;
                    command.Parameters.Add("@DetailedResult", SqlDbType.VarChar, -1).Value = SqlHelper.WriteNullableString(poco.DetailedResult);
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateBirthdayTestDetail(BirthdayTestDetailPoco poco)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateBirthdayTestDetail]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = poco.SimulationId;
                    command.Parameters.Add("@NumberOfDuplicates", SqlDbType.Int).Value = poco.NumberOfDuplicates;
                    command.Parameters.Add("@ExpectedNumberOfDuplicates", SqlDbType.Float).Value = poco.ExpectedNumberOfDuplicates;
                    command.Parameters.Add("@ActualNumberOfDuplicates", SqlDbType.Int).Value = poco.ActualNumberOfDuplicates;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
