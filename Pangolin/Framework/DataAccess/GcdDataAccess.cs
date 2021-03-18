using EnderPi.Framework.Pocos;
using System.Data.SqlClient;
using System.Data;

namespace EnderPi.Framework.DataAccess
{
    public class GcdDataAccess : IGcdDataAccess
    {
        private string _connectionString;

        public GcdDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateGcdTest(GcdTestPoco gcdTest)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateGcdTest]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = gcdTest.SimulationId;
                    command.Parameters.Add("@NumberOfGcds", SqlDbType.BigInt).Value = gcdTest.NumberOfGcds;
                    command.Parameters.Add("@PValueGcd", SqlDbType.Float).Value = gcdTest.PValueGcd;
                    command.Parameters.Add("@TestResultGcd", SqlDbType.Int).Value = (int)gcdTest.TestResultGcd;
                    command.Parameters.Add("@PValueSteps", SqlDbType.Float).Value = gcdTest.PValueSTeps;
                    command.Parameters.Add("@TestResultSteps", SqlDbType.Int).Value = (int)gcdTest.TestResultSteps;
                    command.Parameters.Add("@DetailedResult", SqlDbType.VarChar, -1).Value = SqlHelper.WriteNullableString(gcdTest.DetailedResult);
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateGcdChiSquared(GcdChiSquaredPoco gcdChiSquaredPoco)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[CreateGcdTestChiSquared]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SimulationId", SqlDbType.Int).Value = gcdChiSquaredPoco.SimulationId;
                    command.Parameters.Add("@Gcd", SqlDbType.Int).Value = gcdChiSquaredPoco.Gcd;
                    command.Parameters.Add("@Count", SqlDbType.BigInt).Value = gcdChiSquaredPoco.Count;
                    command.Parameters.Add("@ExpectedCOunt", SqlDbType.Float).Value = gcdChiSquaredPoco.ExpectedCount;
                    command.Parameters.Add("@FractionOfChiSquared", SqlDbType.Float).Value = gcdChiSquaredPoco.FractionOfChiSquared;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }

        }



    }
}
