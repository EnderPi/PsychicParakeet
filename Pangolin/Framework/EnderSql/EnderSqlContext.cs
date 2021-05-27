using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Context for sql operations.
    /// </summary>
    public class EnderSqlContext
    {
        public byte[] Buffer { set; get; }

        public int Offset { set; get; }

    }
}
