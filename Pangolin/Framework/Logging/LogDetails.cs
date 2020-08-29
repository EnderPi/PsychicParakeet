using System;
using System.Collections.Generic;

namespace EnderPi.Framework.Logging
{
    /// <summary>
    /// A list of key-value pairs associated with a logentry.
    /// </summary>
    public class LogDetails
    {
        public List<Tuple<string, string>> Values { get; }

        public LogDetails()
        {
            Values = new List<Tuple<string, string>>();
        }

        public void AddDetail(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Values.Add(new Tuple<string, string>(key, value));
        }
    }
}
