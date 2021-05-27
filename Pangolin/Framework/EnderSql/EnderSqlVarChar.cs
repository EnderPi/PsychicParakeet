using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    public class EnderSqlVarChar : EnderSqlDataType
    {
        private int _maxValue;

        private string _value;
        
        //so varchar layout is.....[byte - length of byte string, 255 indicates in LOB][uint32 lob ID if relevant][byte - length of string or uint - length of string][byte array utf8]

        public override EnderSqlDataType Read(byte[] buffer, int offset, EnderSqlContext context)
        {
            //values greater than 254 will be in the large object heap.  Whether or not it is varchar max doesn't affect this, just validation.
            var length = (int)buffer[offset];
            if (length == byte.MaxValue)
            {
                //go fetch it from the large object heap.  Is that per-table or just a gigantic blob?
                //probably should be per-table, else is risks fragmenting.
                //the large object table for a table is a table of uint32 primary key, uint32 length, and BYTES
                //probably need a parameter object since I have no idea what will be in here.
            }
            else
            {
                _value = Encoding.UTF8.GetString(buffer, offset + 1, length);
            }
            throw new NotImplementedException();
            return new EnderSqlUint32(BitConverter.ToUInt32(buffer, offset));
        }

        public override void Write(byte[] buffer, int offset, EnderSqlContext context)
        {
            throw new NotImplementedException();
        }

        public override int WidthOfDataOnPage => throw new NotImplementedException();

        public override int Length => throw new NotImplementedException();
    }
}
