using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// An exception thrown from EnderSql code.
    /// </summary>
    [Serializable]
    public class EnderSqlException :Exception
    {
        public EnderSqlException() { }

        public EnderSqlException(string text):base(text)  { }

        public EnderSqlException(string message, Exception ex):base(message, ex) { }

        public EnderSqlException(SerializationInfo si, StreamingContext sc):base(si, sc)  { }

    }
}
