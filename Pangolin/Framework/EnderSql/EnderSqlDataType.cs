using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    public abstract class EnderSqlDataType
    {
        /// <summary>
        /// How long, in bytes, this thing is on the page.  In the case of an object on the large object heap, this is the length of the reference, not the length of the actual data.  
        /// </summary>
        public abstract int WidthOfDataOnPage { get; }

        /// <summary>
        /// The length, in bytes, of this object (excludes things like references to the large object heap).
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Reads the data type from the given buffer and offset, under the given context.
        /// </summary>
        /// <param name="buffer">The byte buffer to read from (typically a page)</param>
        /// <param name="offset">The offset to start reading from</param>
        /// <param name="context">Context, for when additional actions are required (such as fetching from the large object heap)</param>
        /// <returns>A datatype with the given data in it.</returns>
        public abstract EnderSqlDataType Read(byte[] buffer, int offset, EnderSqlContext context);

        /// <summary>
        /// Writes the data in this data object to the given buffer, under the given context.
        /// </summary>
        /// <param name="buffer">The buffer to write the data to, typically a page buffer.</param>
        /// <param name="offset">The offset to begin writing at.</param>
        /// <param name="context">The context, for when the operation becomes complex (such as writing a large object to the large object heap.)</param>
        public abstract void Write(byte[] buffer, int offset, EnderSqlContext context);
    }
}
