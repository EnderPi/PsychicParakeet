using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// This is the first page of the database.  Aside from the header, it has tons of metadata and references.
    /// </summary>
    public class EnderSqlDatabaseFirstPage
    {

        public string Name { set; get; }

        public ulong RootPageOfTableTable { set; get; }

        public ulong RootPageOfSchemaTable { set; get; }

        public ulong RootPageOfColumnsTable { set; get; }

        public ulong MaximumNumberOfPages { set; get; }

        public byte MaxConcurrency { set; get; }

        public DatabaseStatistics Statistics {set;get;}

    }
}
