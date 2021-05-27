using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// An Unencrypted Instance.  This isn't a database.  It holds databases.
    /// </summary>
    [Serializable]
    public class SqlInstanceUnencrypted
    {
        private List<EnderSqlDatabase> _databases;


    }
}
