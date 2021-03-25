using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using EnderPi.Framework.Simulation.Genetic;
using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Data access class for storing specimen data
    /// </summary>
    public class GeneticSpecimenDataAccess : IGeneticSpecimenDataAccess
    {
        private string _connectionString;

        public GeneticSpecimenDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes the specimen to the database.
        /// </summary>
        /// <param name="specimen"></param>
        /// <param name="geneticSimulationId"></param>
        /// <returns></returns>
        public int CreateSpecimen(RngSpecies specimen, int geneticSimulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[CreateRngSpecimen]", sqlConnection))
                {
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
                    command.Parameters.Add("@StateOneExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(1).Evaluate();
                    command.Parameters.Add("@StateTwoExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(2).Evaluate();
                    command.Parameters.Add("@OutputExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(3).Evaluate();
                    command.Parameters.Add("@SeedOneExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(4).Evaluate();
                    command.Parameters.Add("@SeedTwoExpression", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(5).Evaluate();

                    command.Parameters.Add("@StateOneExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(1).EvaluatePretty();
                    command.Parameters.Add("@StateTwoExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(2).EvaluatePretty();
                    command.Parameters.Add("@OutputExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(3).EvaluatePretty();
                    command.Parameters.Add("@SeedOneExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(4).EvaluatePretty();
                    command.Parameters.Add("@SeedTwoExpressionPretty", SqlDbType.VarChar, -1).Value = specimen.GetTreeRoot(5).EvaluatePretty();

                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void MarkAsConverged(int specimenId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[MarkSpecimenConverged]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = specimenId;                    

                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public RngSpeciesPoco[] ViewConvergedSpecies(int geneticSimulationId)
        {

            throw new NotImplementedException();
        }


    }
}
