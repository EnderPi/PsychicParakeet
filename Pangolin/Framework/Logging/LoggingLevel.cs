using System;

namespace EnderPi.Framework.Logging
{
    /// <summary>
    /// Simple logging level enumeration.
    /// </summary>
    [Flags]
    public enum LoggingLevel : byte
    {
        None = 0,
        Debug = 1,
        Information = 2,
        Warning = 4,
        Error = 8,
        Fatal = 16
    }
}
