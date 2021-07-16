using EnderPi.Framework.Pocos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace EnderPi.Framework.DataAccess
{
    public class MultiplyRotateDataAccess : IMultiplyRotateDataAccess
    {
        private string _connectionString;

        public MultiplyRotateDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RomulTest[] GetNextRowThatNeedsLevel14()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[SelectNextLevel14Romul]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadRomul(command).ToArray();
                }
            }
        }

        private IEnumerable<RomulTest> ReadRomul(SqlCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int MultiplierPos = reader.GetOrdinal("Multiplier");
                int RotatePos = reader.GetOrdinal("Rotate");
                int seedPos = reader.GetOrdinal("Seed");
                int Level1Pos = reader.GetOrdinal("LevelOneFitness");
                int Level2Pos = reader.GetOrdinal("LevelTwoFitness");
                int Level3Pos = reader.GetOrdinal("LevelThreeFitness");
                int Level12Pos = reader.GetOrdinal("LevelTwelveFitness");
                int Level13Pos = reader.GetOrdinal("LevelThirteenFitness");
                int Level14Pos = reader.GetOrdinal("LevelFourteenFitness");
                
                while (reader.Read())
                {
                    yield return new RomulTest()
                    {
                        Id = reader.GetInt32(idPos),
                        Multiplier = Convert.ToUInt64(reader.GetDecimal(MultiplierPos)),
                        Rotate = reader.GetInt16(RotatePos),
                        Seed = Convert.ToUInt64(reader.GetDecimal(seedPos)),
                        LevelOneFitness = reader.GetInt32(Level1Pos),
                        LevelTwoFitness = reader.GetInt32(Level2Pos),
                        LevelThreeFitness = reader.GetInt32(Level3Pos),
                        LevelTwelveFitness = SqlHelper.ReadNullableLong(reader,Level12Pos),
                        LevelThirteenFitness = SqlHelper.ReadNullableLong(reader, Level13Pos),
                        LevelFourteenFitness = SqlHelper.ReadNullableLong(reader, Level14Pos),

                    };
                }
            }
        }

        private IEnumerable<RomulTest> ReadRomul16(SqlCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                int idPos = reader.GetOrdinal("Id");
                int MultiplierPos = reader.GetOrdinal("Multiplier");
                int RotatePos = reader.GetOrdinal("Rotate");
                int seedPos = reader.GetOrdinal("Seed");
                int Level1Pos = reader.GetOrdinal("LevelOneFitness");
                int Level2Pos = reader.GetOrdinal("LevelTwoFitness");
                int Level3Pos = reader.GetOrdinal("LevelThreeFitness");
                int Level12Pos = reader.GetOrdinal("LevelTwelveFitness");
                int Level13Pos = reader.GetOrdinal("LevelThirteenFitness");
                int Level14Pos = reader.GetOrdinal("LevelFourteenFitness");

                while (reader.Read())
                {
                    yield return new RomulTest()
                    {
                        Id = reader.GetInt32(idPos),
                        Multiplier = Convert.ToUInt64(reader.GetInt32(MultiplierPos)),
                        Rotate = reader.GetInt16(RotatePos),
                        Seed = Convert.ToUInt64(reader.GetDecimal(seedPos)),
                        LevelOneFitness = reader.GetInt32(Level1Pos),
                        LevelTwoFitness = reader.GetInt32(Level2Pos),
                        LevelThreeFitness = reader.GetInt32(Level3Pos),
                        LevelTwelveFitness = SqlHelper.ReadNullableLong(reader, Level12Pos),
                        LevelThirteenFitness = SqlHelper.ReadNullableLong(reader, Level13Pos),
                        LevelFourteenFitness = SqlHelper.ReadNullableLong(reader, Level14Pos),

                    };
                }
            }
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

        public void InsertResult(RomulTest test)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[InsertMultiplyRotate64]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var multiplyParam = command.Parameters.Add("@Multiplier", SqlDbType.Decimal);
                    multiplyParam.Precision = 20;
                    multiplyParam.Value = test.Multiplier;
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = test.Rotate;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = test.Seed;
                    command.Parameters.Add("@LevelOneFitness", SqlDbType.Int).Value = test.LevelOneFitness;
                    command.Parameters.Add("@LevelTwoFitness", SqlDbType.Int).Value = test.LevelTwoFitness;
                    command.Parameters.Add("@LevelThreeFitness", SqlDbType.Int).Value = test.LevelThreeFitness;
                    command.Parameters.Add("@LevelTwelveFitness", SqlDbType.BigInt).Value = test.LevelTwelveFitness;
                    command.Parameters.Add("@LevelThirteenFitness", SqlDbType.BigInt).Value = test.LevelThirteenFitness;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertResult32(RomulTest test)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[InsertMultiplyRotate32Randomness]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.BigInt).Value = test.Multiplier;
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = test.Rotate;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = test.Seed;
                    command.Parameters.Add("@LevelOneFitness", SqlDbType.Int).Value = test.LevelOneFitness;
                    command.Parameters.Add("@LevelTwoFitness", SqlDbType.Int).Value = test.LevelTwoFitness;
                    command.Parameters.Add("@LevelThreeFitness", SqlDbType.Int).Value = test.LevelThreeFitness;
                    command.Parameters.Add("@LevelTwelveFitness", SqlDbType.BigInt).Value = test.LevelTwelveFitness;
                    command.Parameters.Add("@LevelThirteenFitness", SqlDbType.BigInt).Value = test.LevelThirteenFitness;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool RomulExists(ulong multiplier, int rotate, ulong seed)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MultiplyRotate64Exists]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var multiplyParam = command.Parameters.Add("@Multiplier", SqlDbType.Decimal);
                    multiplyParam.Precision = 20;
                    multiplyParam.Value = multiplier;
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = rotate;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = seed;

                    sqlConnection.Open();
                    var result = command.ExecuteScalar();
                    return result != null;
                }
            }
        }

        public bool RomulExists32(ulong multiplier, int rotate, ulong seed)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MultiplyRotate32RandomnessExists]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.BigInt).Value = multiplier;
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = rotate;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = seed;

                    sqlConnection.Open();
                    var result = command.ExecuteScalar();
                    return result != null;
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

        public void UpdateLevel14Test(RomulTest test)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[UpdateRomulLevel14]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Fitness", SqlDbType.BigInt).Value = test.LevelFourteenFitness;
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = test.Id;

                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool RowExists16(int multiplier, int rotate)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[MultiplyRotate16Exists]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.Int).Value = Convert.ToInt32(multiplier);
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = Convert.ToInt32(rotate);

                    sqlConnection.Open();
                    var result = command.ExecuteScalar();
                    return result != null;
                }
            }
        }

        public void CreateRomul16Row(RomulTest test)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[InsertMultiplyRotate16]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Multiplier", SqlDbType.Int).Value = test.Multiplier;
                    command.Parameters.Add("@Rotate", SqlDbType.SmallInt).Value = test.Rotate;
                    var seedParam = command.Parameters.Add("@Seed", SqlDbType.Decimal);
                    seedParam.Precision = 20;
                    seedParam.Value = test.Seed;
                    command.Parameters.Add("@LevelOneFitness", SqlDbType.Int).Value = test.LevelOneFitness;
                    command.Parameters.Add("@LevelTwoFitness", SqlDbType.Int).Value = test.LevelTwoFitness;
                    command.Parameters.Add("@LevelThreeFitness", SqlDbType.Int).Value = test.LevelThreeFitness;
                    command.Parameters.Add("@LevelTwelveFitness", SqlDbType.BigInt).Value = test.LevelTwelveFitness;
                    command.Parameters.Add("@LevelThirteenFitness", SqlDbType.BigInt).Value = test.LevelThirteenFitness;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public RomulTest[] GetRowsFor16BitFillIn()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[Select16BitforFillin]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadRomul16(command).ToArray();
                }
            }
        }

        public void UpdateRomul16Row(RomulTest row)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Simulations].[Update16Romul]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;                    
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = row.Id;
                    command.Parameters.Add("@Gorilla8Fitness", SqlDbType.Int).Value = row.Gorilla8Fitness;
                    command.Parameters.Add("@Gorilla16Fitness", SqlDbType.Int).Value = row.Gorilla16Fitness;
                    command.Parameters.Add("@GcdFitness", SqlDbType.Int).Value = row.GcdFitness;
                    command.Parameters.Add("@BirthdayFitness", SqlDbType.Int).Value = row.BirthdayFitness;
                    command.Parameters.Add("@Maurer16Fitness", SqlDbType.Int).Value = row.Maurer16Fitness;
                    command.Parameters.Add("@Maurer8Fitness", SqlDbType.Int).Value = row.Maurer8Fitness;

                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
