using EnderPi.Framework.Logging;
using System;

namespace EnderPi.Framework.Interfaces
{
    public interface ILogDataAccess
    {
        public void WriteLogRecord(LogMessage logMessage);
        public void WriteLogRecord(LogMessage logMessage, LogDetails details);

        public LogDetails GetLogDetails(long Id);

        public LogMessage[] SearchLogMessages(LogSearchModel searchModel);

    }
}
