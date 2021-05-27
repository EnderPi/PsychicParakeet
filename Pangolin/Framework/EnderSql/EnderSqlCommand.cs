using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    public class EnderSqlCommand
    {
        private EnderSqlConnection _connection;
        private string _commandText;

        public EnderSqlCommand(string command, EnderSqlConnection connection)
        {
            _connection = connection;


        }

        /// <summary>
        /// The general API - execute a command, get a table.
        /// </summary>
        /// <returns></returns>
        public EnderSqlTable Execute()
        {
            //
            throw new NotImplementedException();
        }


    }
}
