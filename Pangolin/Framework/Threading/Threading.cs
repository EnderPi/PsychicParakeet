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
    }
}
