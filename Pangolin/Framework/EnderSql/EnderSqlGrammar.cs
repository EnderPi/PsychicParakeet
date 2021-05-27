using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Static API for EnderSql
    /// </summary>
    public static class EnderSqlGrammar
    {
        private static XmlSerializer _encryptedSerializer = new XmlSerializer(typeof(SqlInstanceEncrypted));

        private static XmlSerializer _unencryptedSerializer = new XmlSerializer(typeof(SqlInstanceUnencrypted));

        /// <summary>
        /// So when the instance starts, it loads one of these from file, then runs commands against the instance.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SqlInstanceUnencrypted OpenSqlInstance(string filename, string key = null)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"EnderSqlInstance with filename {filename} not found!");
            }
            
            var encryptedInstance = new SqlInstanceEncrypted();
            using (var filestream = File.OpenRead(filename))
            {
                encryptedInstance = (SqlInstanceEncrypted)_encryptedSerializer.Deserialize(filestream);
            }

            if (encryptedInstance == null)
            {
                throw new EnderSqlException(EnderSqlErrorCodes.SqlInstanceNotFound);
            }

            var sqlInstance = new SqlInstanceUnencrypted();
            string unencrypted = encryptedInstance.SerializedInstance;
            if (encryptedInstance.Encrypted)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new EnderSqlException();
                }
                else
                {
                    unencrypted = Decrypt(unencrypted, key);                    
                }
            }

            using (StringReader sr = new StringReader(unencrypted))
            {
                sqlInstance = (SqlInstanceUnencrypted)_unencryptedSerializer.Deserialize(sr);
            }

            return sqlInstance;
        }



        private static string Decrypt(string unencrypted, string key)
        {
            return unencrypted;
            //todo IMPLEMENT
        }

        public static void CreateDatabase(string name, string filename, SqlInstanceUnencrypted instance)
        {
            //database must be added to instance.
            //instance must be persisted to file
            //

        }




    }
}
