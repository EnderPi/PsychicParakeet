using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// POCO for application settings.
    /// </summary>
    public class ApplicationSetting
    {
        public string ApplicationName { set; get; }
        public string SettingName { set; get; }
        public string SettingValue { set; get; }
    }
}
