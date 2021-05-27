using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Database type for an unsigned int.
    /// </summary>
    public class EnderSqlUint32 :EnderSqlDataType
    {
        private uint _value;

        public override int WidthOfDataOnPage => 4;

        public override int Length { get => 4; }

        public override EnderSqlDataType Read(byte[] buffer, int offset, EnderSqlContext context)
        {
            return new EnderSqlUint32(BitConverter.ToUInt32(buffer, offset));
        }

        public override void Write(byte[] buffer, int offset, EnderSqlContext context)
        {
            throw new NotImplementedException();
        }

        public EnderSqlUint32(uint value)
        {
            _value = value;
        }



    }
}
