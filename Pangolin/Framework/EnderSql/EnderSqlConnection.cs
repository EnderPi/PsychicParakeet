using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// This is how you connect to the database.  Building one creates a session, logs you in, etc.
    /// </summary>
    public class EnderSqlConnection :IDisposable
    {
        /// <summary>
        /// Logs the user in and get a session token.
        /// </summary>
        /// <param name="connectionString"></param>
        public EnderSqlConnection(string connectionString)
        {
            //make webapi call to log in and get a session
            //TODO set connection timeout.
        }

        /// <summary>
        /// Executes the command and returns 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal object ExecuteCommand(EnderSqlCommand command)
        {
            //Make a web api call with the current session and command.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs the user out, destroys the session.
        /// </summary>
        public void Dispose()
        {
            //make web api call to dispose session
            throw new NotImplementedException();
        }
    }
}
