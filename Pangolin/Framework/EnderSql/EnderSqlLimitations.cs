using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Class to store limitations of EnderSql or constants.
    /// </summary>
    public static class EnderSqlLimitations
    {
        public const ulong PageSize = 65536;

        public const ulong MaimumPagesInDatabase = 4294967296;

        public const ulong MaximumDatabaseSize = 281474976710656;

        public const ulong MaximumColumnsInTable = 256;

        public const ulong MaximumNumberOfTables = 65536;

        public const ulong MaximumRowSize = 65000;
    }
}
