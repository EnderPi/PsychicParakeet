using EnderPi.Framework.Simulation.Genetic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace EnderPi.Framework.DataAccess
{
    public class FeistelSpecimenDataAccess : IFeistelSpecimenDataAccess
    {
        private string _connectionString;

        public FeistelSpecimenDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes the specimen to the database.
        /// </summary>
        /// <param name="specimen"></param>
        /// <param name="geneticSimulationId"></param>
        /// <returns></returns>
        public int CreateSpecimen(RngSpecies32Feistel specimen, int geneticSimulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[CreateFeistelSpecimen]", sqlConnection))
                {
                    SqlParameter parameter;
                    parameter = command.Parameters.AddWithValue("@FeistelKey", CreateDataTable(specimen));
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[GeneticRng].[Feistel32Key]";

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@GeneticSimulationId", SqlDbType.Int).Value = geneticSimulationId;
                    command.Parameters.Add("@Generation", SqlDbType.Int).Value = specimen.Generation;
                    command.Parameters.Add("@Fitness", SqlDbType.Int).Value = specimen.Fitness;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = specimen.Seed;
                    command.Parameters.Add("@Converged", SqlDbType.Bit).Value = 0;
                    command.Parameters.Add("@NumberOfNodes", SqlDbType.Int).Value = specimen.NodeCount;
                    command.Parameters.Add("@Cost", SqlDbType.Float).Value = specimen.TotalCost;
                    command.Parameters.Add("@Rounds", SqlDbType.Int).Value = specimen.Rounds;
                    command.Parameters.Add("@Avalanche", SqlDbType.Float).Value = specimen.AvalancheResults.Avalanche;
                    command.Parameters.Add("@AvalancheRange", SqlDbType.Float).Value = specimen.AvalancheResults.AvalancheRange;
                    command.Parameters.Add("@OutputExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot().Evaluate();
                    command.Parameters.Add("@OutputExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot().EvaluatePretty();

                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private static DataTable CreateDataTable(RngSpecies32Feistel specimen)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("Key", typeof(long));
            for (int i = 0; i < specimen.Keys.Length; i++)
            {
                table.Rows.Add(i, specimen.Keys[i]);
            }

            return table;
        }

        public void MarkAsConverged(int specimenId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[MarkFeistelSpecimenConverged]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = specimenId;

                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
