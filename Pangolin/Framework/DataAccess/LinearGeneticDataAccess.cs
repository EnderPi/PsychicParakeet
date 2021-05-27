using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using EnderPi.Framework.Simulation.LinearGenetic;

namespace EnderPi.Framework.DataAccess
{
    public class LinearGeneticDataAccess : ILinearGeneticDataAccess
    {
        private string _connectionString;

        public LinearGeneticDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes the specimen to the database.
        /// </summary>
        /// <param name="specimen"></param>
        /// <param name="geneticSimulationId"></param>
        /// <returns></returns>
        public int CreateSpecimen(LinearGeneticSpecimen specimen, int geneticSimulationId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[CreateLinearRngSpecimen]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@GeneticSimulationId", SqlDbType.Int).Value = geneticSimulationId;
                    command.Parameters.Add("@Generation", SqlDbType.Int).Value = specimen.Generation;
                    command.Parameters.Add("@Fitness", SqlDbType.Int).Value = specimen.Fitness;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = specimen.Seed;
                    command.Parameters.Add("@Converged", SqlDbType.Bit).Value = 0;
                    command.Parameters.Add("@NumberOfLines", SqlDbType.Int).Value = specimen.ProgramLength;
                    command.Parameters.Add("@SeedProgram", SqlDbType.VarChar, -1).Value = LinearGeneticHelper.PrintProgram(specimen.SeedProgram);
                    command.Parameters.Add("@GenerationProgram", SqlDbType.VarChar, -1).Value = LinearGeneticHelper.PrintProgram(specimen.GenerationProgram);

                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void MarkAsConverged(int specimenId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[MarkLinearSpecimenConverged]", sqlConnection))
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
