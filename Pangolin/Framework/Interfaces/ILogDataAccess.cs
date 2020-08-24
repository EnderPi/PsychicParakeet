using EnderPi.Framework.Logging;

namespace EnderPi.Framework.Interfaces
{
    public interface ILogDataAccess
    {
        public void WriteLogRecord(LogMessage logMessage);
    }
}
