using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnderPi.Framework.DataAccess;
using NUnit.Framework;

namespace UnitTest.Framework
{
    public class ConfigurationTest
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void TestGlobalSettings()
        {
            string settingName = Guid.NewGuid().ToString();
            string settingValue = Guid.NewGuid().ToString();
            string settingValue2 = Guid.NewGuid().ToString();
            ConfigurationDataAccess dataAccess = new ConfigurationDataAccess(Globals.ConnectionString);
            dataAccess.CreateGlobalSetting(settingName, settingValue);
            var setting = dataAccess.GetGlobalSetting(settingName);
            Assert.IsTrue(setting.SettingValue == settingValue);    //Implicitly tests create and read
            dataAccess.UpdateGlobalSetting(settingName, settingValue2);
            setting = dataAccess.GetGlobalSetting(settingName);
            Assert.IsTrue(setting.SettingValue == settingValue2);      //Tests update
            dataAccess.DeleteGlobalSetting(settingName);
            setting = dataAccess.GetGlobalSetting(settingName);
            Assert.IsNull(setting);                                     //Tests delete
        }


        [Test]
        public void TestApplicationSettings()
        {
            string applicationName = Guid.NewGuid().ToString();
            string settingName = Guid.NewGuid().ToString();
            string settingValue = Guid.NewGuid().ToString();
            string settingValue2 = Guid.NewGuid().ToString();
            ConfigurationDataAccess dataAccess = new ConfigurationDataAccess(Globals.ConnectionString);
            dataAccess.CreateApplicationSetting(applicationName, settingName, settingValue);
            var setting = dataAccess.GetApplicationSettings(applicationName).FirstOrDefault(x=>x.SettingName == settingName);
            Assert.IsTrue(setting.SettingValue == settingValue);    //Implicitly tests create and read
            dataAccess.UpdateApplicationSettingValue(applicationName, settingName, settingValue2);
            setting = dataAccess.GetApplicationSettings(applicationName).FirstOrDefault(x => x.SettingName == settingName);
            Assert.IsTrue(setting.SettingValue == settingValue2);      //Tests update
            dataAccess.DeleteApplicationSetting(applicationName, settingName);
            setting = dataAccess.GetApplicationSettings(applicationName).FirstOrDefault(x => x.SettingName == settingName);
            Assert.IsNull(setting);                                     //Tests delete
        }

    }
}
