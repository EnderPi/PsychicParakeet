using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// This is kinda what queries return.
    /// </summary>
    public class EnderSqlTable
    {
        public List<EnderSqlTableRow> _rows;

        public List<EnderSqlColumn> _columnDefinitions;
    }
}
