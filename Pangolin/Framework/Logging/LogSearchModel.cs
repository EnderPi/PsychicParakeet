using System;
using System.ComponentModel.DataAnnotations;

namespace EnderPi.Framework.Logging
{
    /// <summary>
    /// POCO for searching the logs.
    /// </summary>
    public class LogSearchModel
    {
        [StringLength(100, MinimumLength = 1)]
        public string Source { set; get; }

        public DateTime BeginTime { set; get; }

        public DateTime EndTime { set; get; }

        public string Message { set; get; }

        public bool ShowDebug { set; get; }

        public bool ShowInformation { set; get; }

        public bool ShowWarning { set; get; }

        public bool ShowError { set; get; }

        public bool ShowFatal { set; get; }
    }
}
