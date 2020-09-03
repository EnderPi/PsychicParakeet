using EnderPi.Framework.Interfaces;
using System;
using System.Threading.Tasks;

namespace EnderPi.Framework.Logging
{
    /// <summary>
    /// A logging class.  This is intended to be a stateful, Dependency-Injection class.  A single instance should be created and passed around the application.
    /// </summary>
    /// <remarks>
    /// All methods are thread safe, and logging will never throw, although the class constructor may.
    /// </remarks>
    public class Logger
    {
        /// <summary>
        /// Reference to the data access object used to write to the database.
        /// </summary>
        private ILogDataAccess _logDataAccess;

        /// <summary>
        /// The log levels that this will log.
        /// </summary>
        private LoggingLevel _levelsToLog;

        /// <summary>
        /// The source associated with this logger instance.
        /// </summary>
        private string _source;

        /// <summary>
        /// Basic constructor.  Stores relevant state, and hooks into the unhandled exception event for the AppDomain.
        /// </summary>
        /// <param name="logDataAccess"></param>
        /// <param name="source"></param>
        /// <param name="levelsToLog"></param>
        public Logger(ILogDataAccess logDataAccess, string source, LoggingLevel levelsToLog)
        {
            if (logDataAccess == null)
            {
                throw new ArgumentNullException(nameof(logDataAccess));
            }
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentOutOfRangeException(nameof(source));
            }
            if (levelsToLog == LoggingLevel.None)
            {
                throw new ArgumentOutOfRangeException(nameof(levelsToLog));
            }
            _logDataAccess = logDataAccess;
            _source = source;
            _levelsToLog = levelsToLog;
            if ((_levelsToLog & LoggingLevel.Fatal) == LoggingLevel.Fatal)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += UnhandledExceptionHandler;
            }
        }

        /// <summary>
        /// Logs the exception to the log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                {
                    LogMessage logMessage = new LogMessage(0, _source, DateTime.Now, LoggingLevel.Fatal, ex.ToString());
                    _logDataAccess.WriteLogRecord(logMessage);
                }
            }
            catch(Exception)
            { }            
        }

        /// <summary>
        /// Logs the given message to the source associated with this logger if this logger is set to log the given level.
        /// </summary>
        /// <remarks>
        /// Will not log empty messages or those with no logging level.
        /// </remarks>
        /// <param name="message">The string statement to write to the log.</param>
        /// <param name="level">The logging level associated with this message.</param>
        /// <exception cref="">None</exception>
        public void Log(string message, LoggingLevel level, LogDetails details)
        {
            Task.Run(() => LogPrivate(message, level, details));
        }


        public void Log(string message, LoggingLevel level)
        {
            Log(message, level, null);
        }

        /// <summary>
        /// Delegate purely for running logging asynchronously.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="details"></param>
        private void LogPrivate(string message, LoggingLevel level, LogDetails details)
        {
            if (!string.IsNullOrWhiteSpace(message) && level != LoggingLevel.None)
            {
                try
                {
                    if ((_levelsToLog & level) == level)
                    {
                        LogMessage logMessage = new LogMessage(0, _source, DateTime.Now, level, message);
                        _logDataAccess.WriteLogRecord(logMessage, details);
                    }
                }
                catch (Exception)
                { }
            }
        }

    }
}
