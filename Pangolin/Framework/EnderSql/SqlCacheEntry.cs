using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    public class SqlCacheEntry
    {
        public EnderSqlPage Page { set; get; }
        public DateTime LastRequested { set; get; }
    }
}
