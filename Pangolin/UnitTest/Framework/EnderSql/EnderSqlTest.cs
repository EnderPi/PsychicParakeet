using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using EnderPi.Framework.EnderSql;

namespace UnitTest.Framework.EnderSql
{
    public class EnderSqlTest
    {

        [Test]
        public void TestCreateSchema()
        {
            string connectionString = "";
            using (EnderSqlConnection connection = new EnderSqlConnection(connectionString))
            {

            }

        }
    }
}