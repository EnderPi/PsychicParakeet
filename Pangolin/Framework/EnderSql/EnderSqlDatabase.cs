using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// This is a database.
    /// </summary>
    public class EnderSqlDatabase
    {
        private string _fileName;
        private string _encryptionKey;
        /// <summary>
        /// Database loads this up preemptively.
        /// </summary>
        private Dictionary<string, ulong> _tableRootPageNumbers;

        private LockManager _lockManager;

        private SqlCacheManager _cacheManager;

        private SqlTransactionManager _sqlTransactionManager;

        //force a pause and flush of this if it gets too big, like over 10% of max cache size.
        private Queue<uint> _dirtyPages;

        /// <summary>
        /// So you create a database with a filename and encryption key, which makes it load the metadata from the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        public EnderSqlDatabase(string fileName, string databaseName, string key)
        {
            //confirm file exists
            //load the first page, unencrypt
            //verify the name on the first page is the name of the db, verify page number is zero.
            //populate metadata on this object
            //load table root page numbers.
            LoadRootTableData();

        }

        private void LoadRootTableData()
        {
            _tableRootPageNumbers = new Dictionary<string, ulong>();
            //so this is going to be a good example of a select *.
            //run a select * from the table table, read it into the object.

            //build a table definition to execute the query.
            //this table is unique in that it needs to be loaded before anything else, so the table layout is hardcoded.
            var tableReader = new EnderSqlTableReader();
            tableReader.AddColumn("Id", typeof(EnderSqlUint32));
            tableReader.AddColumn("TableName", typeof(EnderSqlVarChar), 128);

            //load root page for table table
            //walk down to leaf
            //traverse leaves.
            throw new NotImplementedException();
        }

        public EnderSqlSession GetConnection(EnderSqlCredentials cred)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// In some sense, this is the only public API exposed by the database.  You execute a query against it.  
        /// TODO manage connection pooling, identity, security, etc.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public EnderSqlTable ExecuteQuery(EnderSqlSession session, EnderSqlCommand command)
        {
            //so lots of scaffoding needed here
            //first things first, need to parse the command into something so we can do authorization.



            //authori
            //start timer
            //performance metrics.
            //parse incoming request

            //Hard stop and purge dirty pages if needed.

            //so this is a tough one.  Going to have to start simple since I will need a whole parsing engine.
            throw new NotImplementedException();
        }

        




    }
}
