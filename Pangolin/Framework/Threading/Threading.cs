using EnderPi.Framework.Logging;
using System;

namespace EnderPi.Framework.Threading
{
    //todo fix this, I really shouldn't clash with .NET namespaces
    public static class Threading
    {
        /// <summary>
        /// Executes the given delegate in a try-catch that suppresses any exception.
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteWithoutThrowing(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Executes the given delegate in a try-catch that suppresses any exception, but logs the exception to the given logger.
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteWithoutThrowing(Action action, Logger logger)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString(), LoggingLevel.Error);
            }
        }


    }
}
