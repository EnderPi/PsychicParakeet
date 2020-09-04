using EnderPi.Framework.Pocos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// CRUD for global and application settings.
    /// </summary>
    public class ConfigurationDataAccess
    {
        private string _connectionString;
        
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="connectionString">Connection string for the database.</param>
        public ConfigurationDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateGlobalSetting(string key, string value)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[CreateGlobalSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    command.Parameters.Add("@SettingValue", SqlDbType.VarChar, -1).Value = value;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateGlobalSetting(string key, string value)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[UpdateGlobalSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    command.Parameters.Add("@SettingValue", SqlDbType.VarChar, -1).Value = value;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public GlobalSetting GetGlobalSetting(string key)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[GetGlobalSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;                    
                    sqlConnection.Open();
                    return ReadGlobalSettings(command).FirstOrDefault();
                }
            }
        }

        public GlobalSetting[] GetGlobalSettings()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[GetGlobalSettings]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadGlobalSettings(command).ToArray();
                }
            }
        }

        private IEnumerable<GlobalSetting> ReadGlobalSettings(SqlCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                int keyPos = reader.GetOrdinal("Name");
                int valuePos = reader.GetOrdinal("Value");
                while (reader.Read())
                {
                    yield return new GlobalSetting() { SettingName = reader.GetString(keyPos), SettingValue = reader.GetString(valuePos) };
                }
            }
        }

        public void DeleteGlobalSetting(string key)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[DeleteGlobalSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        public void CreateApplicationSetting(string applicationName, string key, string value)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[CreateApplicationSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = applicationName;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    command.Parameters.Add("@SettingValue", SqlDbType.VarChar, -1).Value = value;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateApplicationSettingValue(string applicationName, string key, string value)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[UpdateApplicationSettingValue]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = applicationName;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    command.Parameters.Add("@SettingValue", SqlDbType.VarChar, -1).Value = value;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public ApplicationSetting[] GetApplicationSettings(string applicationName)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[GetApplicationSettingsByApplication]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = applicationName;
                    sqlConnection.Open();
                    return ReadApplicationSettings(command).ToArray();
                }
            }
        }

        private IEnumerable<ApplicationSetting> ReadApplicationSettings(SqlCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                int appPos = reader.GetOrdinal("Application");
                int keyPos = reader.GetOrdinal("Name");
                int valuePos = reader.GetOrdinal("Value");
                while (reader.Read())
                {
                    yield return new ApplicationSetting() { ApplicationName = reader.GetString(appPos), SettingName = reader.GetString(keyPos), SettingValue = reader.GetString(valuePos) };
                }
            }
        }

        public ApplicationSetting[] GetApplicationSettings()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[GetApplicationSettings]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    return ReadApplicationSettings(command).ToArray();
                }
            }
        }

        public void DeleteApplicationSetting(string applicationName, string settingName)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[DeleteApplicationSetting]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = applicationName;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = settingName;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }                
    }
}
