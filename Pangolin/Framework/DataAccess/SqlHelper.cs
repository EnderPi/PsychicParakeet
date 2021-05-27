using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace EnderPi.Framework.DataAccess
{
    public static class SqlHelper
    {
        private static string SqlDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public static object WriteNullableDateTime(DateTime dateTime)
        {
            return (dateTime == DateTime.MinValue) ? DBNull.Value : (object)dateTime;
        }

        public static object WriteNullableDateTime(DateTime? dateTime)
        {
            return (dateTime.HasValue) ? (object)dateTime : DBNull.Value;
        }

        public static object WriteNullableString(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? DBNull.Value : (object)s;
        }

        public static string ReadNullableString(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);            
        }

        public static DateTime? ReadNullableDateTime(SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Converts the given time to a sql date time format appropriate for queries.  COnverts values less than sqldatetime Min to sqldatetimeMin.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToSqlDateTimeString(DateTime dateTime)
        {
            if (dateTime < SqlDateTime.MinValue)
            {
                return ((DateTime)SqlDateTime.MinValue).ToString(SqlDateTimeFormat);
            }
            else
            {
                return dateTime.ToString(SqlDateTimeFormat);
            }
        }

    }
}
