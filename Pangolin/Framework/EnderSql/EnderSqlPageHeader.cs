using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// The header on every page.  256 bytes planned.
    /// </summary>
    public class EnderSqlPageHeader
    {
        public ulong PageNumber { set; get; }

        public ulong TableId { set; get; }

        public bool IsIndex { set; get; }

        public bool IsLeaf { set; get; }

        public bool IsRoot { set; get; }

        public bool IsEmpty { set; get; }

        public ulong NextPageAtThisLevel { set; get; }

        public ulong PreviousPageAtThisLevel { set; get; }

        public ulong ParentPage { set; get; }

    }
}
