using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// This is a SQL Instance.  Might have several databases.  
    /// </summary>
    /// <remarks>
    /// This thing is serialized to a file that is loaded when the instance starts.
    /// </remarks>
    [Serializable]
    public class SqlInstanceEncrypted
    {
        public SqlInstanceEncrypted()
        {
            //if file exists, load and unencrypt if needed, then create an unencrypted instance.
        }

        public bool Encrypted { set; get; }

        public string SerializedInstance { set; get; }

        

    }
}
