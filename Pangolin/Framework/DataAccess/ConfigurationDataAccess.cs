using EnderPi.Framework.Pocos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using EnderPi.Framework.Logging;
using EnderPi.Framework.Messaging.Events;
using EnderPi.Framework.Caching;
using EnderPi.Framework.Interfaces;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// CRUD for global and application settings.  Probably needs split.
    /// </summary>
    /// <remarks>
    /// Probably needs split for global and app, and may need splitting for reading and writing.  This data access class is unique in that
    /// you would like data access classes to have an event manager so that they can send cache invalidation events, but you typically
    /// can't construct an event manager without first reading global and application settings to get the queue names.
    /// So for these data access classes only, might need a reader so that you can get the settings, then another class that writes.
    /// Currently handles this problem by providing a property of event manager you can set, but obviously it just throws if you forget 
    /// to set that prior to writing.
    /// </remarks>
    public class ConfigurationDataAccess : IConfigurationDataAccess
    {
        /// <summary>
        /// The database connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Event manager for making updates and invalidating caches.
        /// </summary>
        public IEventManager EventManager { set; get; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="connectionString">Connection string for the database.</param>
        public ConfigurationDataAccess(string connectionString)
        {
            _connectionString = connectionString;            
        }

        /// <summary>
        /// Creates a global setting.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="value">The value for the setting.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the key or value are null or whitespace.</exception>
        public void CreateGlobalSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
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

        /// <summary>
        /// Updates the global setting of the given key with the given value.
        /// </summary>
        /// <param name="key">The setting to update.</param>
        /// <param name="value">The new value for the setting.</param>
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
            var cacheUpdatedEVent = new CacheInvalidationEvent() { CacheName = CacheNames.GlobalConfigurations, UpdatedKey = key};
            EventManager.PublishEvent(cacheUpdatedEVent, Messaging.MessagePriority.Highest);
        }

        /// <summary>
        /// Gets the global setting with the given key.
        /// </summary>
        /// <param name="key">The key of the global setting to retrive.</param>
        /// <returns>The global setting if it exists, null otherwise.</returns>
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

        /// <summary>
        /// Gets all the global settings.
        /// </summary>
        /// <returns>An array with all of the global settings.</returns>
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

        /// <summary>
        /// Private method for reading global settings from SQL commands.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the global setting with the given key.
        /// </summary>
        /// <param name="key">The key of the global setting to delete.</param>
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

        /// <summary>
        /// Creates an application setting.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <exception cref="ArgumentOutOfRangeException">If applicationName or key are null or whitespace.</exception>
        public void CreateApplicationSetting(string applicationName, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentOutOfRangeException(nameof(applicationName));
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }
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

        /// <summary>
        /// Updates the application setting of the given key with the given new value.
        /// </summary>
        /// <param name="applicationName">The name of the application to update.</param>
        /// <param name="key">The key of the setting to update.</param>
        /// <param name="newValue">The new value for the setting.</param>
        public void UpdateApplicationSettingValue(string applicationName, string key, string newValue)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("[Configuration].[UpdateApplicationSettingValue]", sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 100).Value = applicationName;
                    command.Parameters.Add("@SettingName", SqlDbType.VarChar, 100).Value = key;
                    command.Parameters.Add("@SettingValue", SqlDbType.VarChar, -1).Value = newValue;
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets all of the application settings for the given application name.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <returns>An array of all the application settings for that application.</returns>
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

        /// <summary>
        /// Private method for reading application settings from a SQL command.
        /// </summary>
        /// <param name="command">The command to execute and read.</param>
        /// <returns>IEnumerable of the application settings.</returns>
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

        /// <summary>
        /// Gets all the application settings.
        /// </summary>
        /// <returns>An array of all the application settings.</returns>
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

        /// <summary>
        /// Deletes the application setting for the given application with the given key.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="settingName">The setting name.</param>
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

        /// <summary>
        /// Gets a global setting and converts it to an integer.
        /// </summary>
        /// <param name="name">The name of the global setting.</param>
        /// <param name="defaultValue">The value to return if the setting doesn't exist.</param>
        /// <returns>The setting value, converted to an integer, or the default value.</returns>
        public int GetGlobalSettingInt(string name, int defaultValue = 0)
        {
            int result = defaultValue;
            var setting = GetGlobalSetting(name);
            if (setting != null)
            {
                result = Convert.ToInt32(setting.SettingValue);
            }
            return result;
        }

        /// <summary>
        /// Gets a global setting and returns it as a string.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <returns>The setting as a string, or null if it doesn't exist.</returns>
        public string GetGlobalSettingString(string name)
        {
            string result = null;
            var setting = GetGlobalSetting(name);
            if (setting != null)
            {
                result = setting.SettingValue;
            }
            return result;
        }

        /// <summary>
        /// Fancy reflection, pass it a POCO and it will do it's best to read the settings for the given application and convert them to the appropriate type.
        /// </summary>
        /// <typeparam name="T">The type of the POCO</typeparam>
        /// <param name="applicationName">The application name.</param>
        /// <returns>A new POCO of type T, populated with values based on the property names and the values in the application settings table.</returns>
        /// <remarks>
        /// Currently handles strings, logginglevel, and integers.  Support for addiitonal types must be added manually.
        /// </remarks>
        public T GetApplicationConfigurationValues<T>(string applicationName) where T : new()
        {
            T result = new T();
            var resultType = typeof(T);
            var settings = GetApplicationSettings(applicationName);
            foreach (var property in resultType.GetProperties())
            {
                string name = property.Name;
                var setting = settings.FirstOrDefault(x => string.Equals(name, x.SettingName, StringComparison.OrdinalIgnoreCase));
                string value = null;
                if (setting != null)
                {
                    value = setting.SettingValue;
                }
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(result, Convert.ToInt32(value));
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(result, value);
                }
                else if (property.PropertyType == typeof(LoggingLevel))
                {
                    property.SetValue(result, (LoggingLevel)Convert.ToByte(value));
                }
            }
            return result;
        }

        public double GetGlobalSettingDouble(string name, double defaultValue)
        {
            double result = defaultValue;
            var setting = GetGlobalSetting(name);
            if (setting != null)
            {
                result = Convert.ToDouble(setting.SettingValue);
            }
            return result;
        }
    }
}
