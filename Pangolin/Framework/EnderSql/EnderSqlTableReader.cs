using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Reads rows from a table.  This is an oddball class right now, it reads before table definitions are present.
    /// </summary>
    public class EnderSqlTableReader
    {
        private List<EnderSqlColumn> _columns;

        public EnderSqlTableReader() 
        { 
        }

        public void AddColumn(string columnName, System.Type type, uint length = 0)
        {

        }

        /// <summary>
        /// Reads all rows!
        /// </summary>
        /// <param name="rootPage"></param>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        public EnderSqlTable ReadAllRows(EnderSqlPage rootPage, SqlCacheManager cacheManager)
        {
            //so we have the root page, need to walk down to leaf, then read across leaves!

            //so the rows on this table are the root of a b+ tree
            //each row is (Id, PageNumber)
            //when we get to the leaves, the leaves are (Id(uint), TableName(Varchar256))

            //get row 1
            //read the page number
            //get that page
            //while it's not a leaf, keep going.
            EnderSqlPage page = rootPage;
            do
            {
                int pageOffset = page.GetRowLocation(0)+4;
                uint nextPageNumber = BitConverter.ToUInt32(page.PageData, pageOffset);
                page = cacheManager.GetPage(nextPageNumber);
                //so this location is a uint, need to offset by 4 to get the page

            } while (!page.IsLeaf);

            //so now we have a leaf, need to read across.


            //read all rows on page
            EnderSqlTable resultTable = new EnderSqlTable();
            AddRows(resultTable, page);
            //if there's another page, read more.


            return resultTable;
        }
        private void AddRows(EnderSqlTable resultTable, EnderSqlPage page)
        {
            for (int i=0; i < page.RowCount; i++)
            {
                var rowOffset = page.GetRowLocation(i);
                uint Id = BitConverter.ToUInt32(page.PageData, rowOffset);
                //Varchars have a 2-byte length;
                var nameLength = (int)BitConverter.ToUInt16(page.PageData, rowOffset + 4);
                var nameBytes = new byte[nameLength];
                Buffer.BlockCopy(page.PageData, rowOffset + 6, nameBytes, 0, nameLength);
                //todo get rid of block copy
                string tableName = Encoding.UTF8.GetString(nameBytes);

            }            
        }

        private EnderSqlTableRow ReadRow(byte[] buffer, int offset)
        {
            int columnOffset = offset;
            EnderSqlTableRow row = new EnderSqlTableRow();
            var context = new EnderSqlContext();
            for (int i=0; i < _columns.Count; i++)
            {
                var val = _columns[i].DataType.Read(buffer, columnOffset, context);
                columnOffset += val.WidthOfDataOnPage;
                row.AddValue(val);
            }
            return row;
        }
    }
}
