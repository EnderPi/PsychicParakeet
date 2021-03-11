using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    /// <summary>
    /// Interface for access to tables in the configuration schema.
    /// </summary>
    public interface IConfigurationDataAccess
    {
        /// <summary>
        /// Creates a new application setting.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void CreateApplicationSetting(string applicationName, string key, string value);

        /// <summary>
        /// Create a new global setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void CreateGlobalSetting(string key, string value);

        /// <summary>
        /// Deletes the application setting for the given application with the given key.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="settingName">The setting name.</param>
        void DeleteApplicationSetting(string applicationName, string settingName);

        /// <summary>
        /// Deletes the global setting with the given key.
        /// </summary>
        /// <param name="key">The key of the global setting to delete.</param>
        void DeleteGlobalSetting(string key);

        /// <summary>
        /// Fancy reflection, pass it a POCO and it will do it's best to read the settings for the given application and convert them to the appropriate type.
        /// </summary>
        /// <typeparam name="T">The type of the POCO</typeparam>
        /// <param name="applicationName">The application name.</param>
        /// <returns>A new POCO of type T, populated with values based on the property names and the values in the application settings table.</returns>
        /// <remarks>
        /// Currently handles strings, logginglevel, and integers.  Support for addiitonal types must be added manually.
        /// </remarks>
        T GetApplicationConfigurationValues<T>(string applicationName) where T : new();

        /// <summary>
        /// Gets all the application settings.
        /// </summary>
        /// <returns>An array of all the application settings.</returns>
        ApplicationSetting[] GetApplicationSettings();

        /// <summary>
        /// Gets all of the application settings for the given application name.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <returns>An array of all the application settings for that application.</returns>
        ApplicationSetting[] GetApplicationSettings(string applicationName);

        /// <summary>
        /// Gets the global setting with the given key.
        /// </summary>
        /// <param name="key">The key of the global setting to retrive.</param>
        /// <returns>The global setting if it exists, null otherwise.</returns>
        GlobalSetting GetGlobalSetting(string key);

        /// <summary>
        /// Gets a global setting and converts it to an integer.
        /// </summary>
        /// <param name="name">The name of the global setting.</param>
        /// <param name="defaultValue">The value to return if the setting doesn't exist.</param>
        /// <returns>The setting value, converted to an integer, or the default value.</returns>
        int GetGlobalSettingInt(string name, int defaultValue);

        /// <summary>
        /// Gets all the global settings.
        /// </summary>
        /// <returns>An array with all of the global settings.</returns>
        GlobalSetting[] GetGlobalSettings();

        /// <summary>
        /// Gets a global setting and returns it as a string.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <returns>The setting as a string, or null if it doesn't exist.</returns>
        string GetGlobalSettingString(string name);

        /// <summary>
        /// Updates the application setting of the given key with the given new value.
        /// </summary>
        /// <param name="applicationName">The name of the application to update.</param>
        /// <param name="key">The key of the setting to update.</param>
        /// <param name="newValue">The new value for the setting.</param>
        void UpdateApplicationSettingValue(string applicationName, string key, string value);
        double GetGlobalSettingDouble(string geneticTournamentProbability, double v);

        /// <summary>
        /// Updates the global setting of the given key with the given value.
        /// </summary>
        /// <param name="key">The setting to update.</param>
        /// <param name="value">The new value for the setting.</param>
        void UpdateGlobalSetting(string key, string value);
    }
}