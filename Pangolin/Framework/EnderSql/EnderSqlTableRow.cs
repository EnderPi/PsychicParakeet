using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Abstracted class for a row in a table.
    /// </summary>
    public class EnderSqlTableRow
    {
        public List<EnderSqlDataType> _items;

        public EnderSqlTableRow()
        {
            _items = new List<EnderSqlDataType>();
        }

        internal void AddValue(EnderSqlDataType data)
        {
            _items.Add(data);
        }
    }
}
