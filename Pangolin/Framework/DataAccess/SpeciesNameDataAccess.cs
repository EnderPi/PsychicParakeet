using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EnderPi.Framework.Random;
using EnderPi.Framework.Pocos;
using System.Linq;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Provides data access to the species names.
    /// </summary>
    public class SpeciesNameDataAccess : ISpeciesNameDataAccess
    {
        private string _connectionString;
        private ThreadsafeEngine _generator;

        public SpeciesNameDataAccess(string connectionString)
        {
            _connectionString = connectionString;
            _generator = new ThreadsafeEngine(new Sha256());
        }

        /// <summary>
        /// Creates a new name.
        /// </summary>
        /// <param name="name"></param>
        public void CreateNewName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[CreateSpeciesName]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 100).Value = name;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the count of names.
        /// </summary>
        /// <returns></returns>
        public int GetNameCount()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[GetNameCount]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets a new random name.
        /// </summary>
        /// <returns></returns>
        public string GetRandomName()
        {
            int count = GetNameCount();
            SpeciesName newName = null;
            do
            {
                int id = _generator.NextInt(1, count);
                newName = GetNameWithNewCounter(id);
            } while (newName == null);
            return $"{newName.Name} #{newName.Counter}";
        }

        /// <summary>
        /// Gets the name with the given ID and increments the counter.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SpeciesName GetNameWithNewCounter(int id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[GetSpeciesName]", sqlConnection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadSpeciesName(command).FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets all names in the table.
        /// </summary>
        /// <returns></returns>
        public List<SpeciesName> GetAllNames()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[GeneticRng].[GetAllSpeciesNames]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadSpeciesName(command).ToList();
                }
            }
        }

        /// <summary>
        /// Method for reading sql commands.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IEnumerable<SpeciesName> ReadSpeciesName(SqlCommand command)
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int namePos = reader.GetOrdinal("Name");
                int counterPos = reader.GetOrdinal("Counter");
                while (reader.Read())
                {
                    var speciesName = new SpeciesName();
                    speciesName.Id = reader.GetInt32(idPos);
                    speciesName.Name = SqlHelper.ReadNullableString(reader, namePos);
                    speciesName.Counter = reader.GetInt32(counterPos);
                    yield return speciesName;
                }
            }
        }


    }
}
