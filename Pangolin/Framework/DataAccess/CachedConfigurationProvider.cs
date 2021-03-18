using EnderPi.Framework.Caching;
using EnderPi.Framework.Pocos;

namespace EnderPi.Framework.DataAccess
{
    public class CachedConfigurationProvider : IConfigurationDataAccess
    {
        private IConfigurationDataAccess _dataAccess;
        private ICache _cache;

        public CachedConfigurationProvider(IConfigurationDataAccess dataAccess, ICache cache)
        {
            _dataAccess = dataAccess;
            _cache = cache;
        }

        public void CreateApplicationSetting(string applicationName, string key, string value)
        {
            _dataAccess.CreateApplicationSetting(applicationName, key, value);
        }

        public void CreateGlobalSetting(string key, string value)
        {
            _dataAccess.CreateGlobalSetting(key, value);
        }

        public void DeleteApplicationSetting(string applicationName, string settingName)
        {
            _dataAccess.DeleteApplicationSetting(applicationName, settingName);
        }

        public void DeleteGlobalSetting(string key)
        {
            _dataAccess.DeleteGlobalSetting(key);
        }

        public T GetApplicationConfigurationValues<T>(string applicationName) where T : new()
        {
            return _cache.Fetch<T>($"GetAppConfigVal_{applicationName}", () => _dataAccess.GetApplicationConfigurationValues<T>(applicationName));
        }

        public ApplicationSetting[] GetApplicationSettings()
        {
            return _cache.Fetch("GetAppSettings", () => _dataAccess.GetApplicationSettings());
        }

        public ApplicationSetting[] GetApplicationSettings(string applicationName)
        {
            return _cache.Fetch($"GetAppSettings_{applicationName}", () => _dataAccess.GetApplicationSettings(applicationName));
        }

        public GlobalSetting GetGlobalSetting(string key)
        {
            return _cache.Fetch($"GetGlobalSetting_{key}", () => _dataAccess.GetGlobalSetting(key));
        }

        public double GetGlobalSettingDouble(string settingName, double defaultValue)
        {
            return _cache.Fetch($"GetGlobalSettingDouble_{settingName}_{defaultValue}", () => _dataAccess.GetGlobalSettingDouble(settingName, defaultValue));
        }

        public int GetGlobalSettingInt(string name, int defaultValue)
        {
            return _cache.Fetch($"GetGlobalSettingInt_{name}_{defaultValue}", () => _dataAccess.GetGlobalSettingInt(name, defaultValue));
        }

        public GlobalSetting[] GetGlobalSettings()
        {
            return _cache.Fetch($"GetGlobalSettings", () => _dataAccess.GetGlobalSettings());
        }

        public string GetGlobalSettingString(string name)
        {
            return _cache.Fetch($"GetGlobalSettingString_{name}", () => _dataAccess.GetGlobalSettingString(name));
        }

        public void UpdateApplicationSettingValue(string applicationName, string key, string value)
        {
            _dataAccess.UpdateApplicationSettingValue(applicationName, key, value);
        }

        public void UpdateGlobalSetting(string key, string value)
        {
            _dataAccess.UpdateGlobalSetting(key, value);
        }
    }
}
