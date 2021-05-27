using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    public class EnderSqlPage
    {
        public const int PageLength = 65536;

        public EnderSqlPage(byte[] bytes)
        {
            if (bytes.Length != PageLength)
            {
                throw new EnderSqlException();
            }
            _pageData = bytes;
        }

        public EnderSqlPage()
        {
            _pageData = new byte[PageLength];
        }

        public byte[] PageData { get { return _pageData; } }

        private byte[] _pageData;

        private void SetBytes(int offset, byte[] bytes) 
        {
            Buffer.BlockCopy(bytes, 0, _pageData, offset, bytes.Length);
            IsDirty = true;
            //set some bytes, then mark dirty.
        }

        /// <summary>
        /// The page number is the first 4 bytes.  That means max 4 billion pages, 256 terabytes.
        /// </summary>
        public uint PageNumber { get { return BitConverter.ToUInt32(_pageData, 0); }  }

        /// <summary>
        /// The table that owns this page.  Bytes 4-7 (5-8)
        /// </summary>
        public uint TableId
        {
            set { SetBytes(4, BitConverter.GetBytes(value)); }
            get { return BitConverter.ToUInt32(_pageData, 4); }
        }

        /// <summary>
        /// Row count is bytes 8-9 
        /// </summary>
        public ushort RowCount
        {
            set { SetBytes(8, BitConverter.GetBytes(value)); }
            get { return BitConverter.ToUInt16(_pageData, 8); }
        }

        /// <summary>
        /// The only page element that doesn't go through the API.
        /// </summary>
        public bool IsDirty
        {
            set { _pageData[10] = BitConverter.GetBytes(value)[0]; }
            get { return BitConverter.ToBoolean(_pageData, 10); }
        }

        /// <summary>
        /// If this page is an index page, this will be the last page at this level.
        /// </summary>
        public uint LastPageReferenceBTreeNode
        {
            set { SetBytes(11, BitConverter.GetBytes(value)); }
            get { return BitConverter.ToUInt32(_pageData, 11); }
        }

        public bool IsIndex { set; get; }

        public bool IsLeaf { set; get; }

        public bool IsRoot { set; get; }

        public bool IsEmpty { set; get; }

        public bool IsLargeObjectPage { set; get; }

        public ushort EmptySpace { set; get; }

        public ulong NextPageAtThisLevel { set; get; }

        public ulong PreviousPageAtThisLevel { set; get; }

        public ulong ParentPage { set; get; }


        //OK, so the row offset array is the last bytes of the page.

        /// <summary>
        /// Gets the offset for the given row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int GetRowLocation(int row)
        {
            //we have to subtract two because row offsets are shorts.
            return BitConverter.ToUInt16(_pageData,(PageLength-2) - (row * 2));
            //row offset is in backwards order at the end of the page.
            //TODO validation?
        }

    }
}
