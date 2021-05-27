using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnderPi.Framework.EnderSql
{
    /// <summary>
    /// Manages disk reads and writes, plus cache pool.  All methods are thread-safe.
    /// </summary>
    public class SqlCacheManager
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The maximum number of pages to keep in the cache.
        /// </summary>
        private int _maxPagesInCache;

        /// <summary>
        /// The number of pages at which the manager will start pruning asynchronously to avoid synchronous blocking.
        /// </summary>
        private int _thresholdPages;

        /// <summary>
        /// The locking object for the cache.
        /// </summary>
        private object _cacheLock;

        /// <summary>
        /// The locking object for disk access.
        /// </summary>
        private object _diskLock;

        /// <summary>
        /// The cache.  Not a concurrent dictionary because dates need to be set and pruning the cache would otherwise be hinky.
        /// </summary>
        private Dictionary<uint, SqlCacheEntry> _cache;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="fileName">The name of the database file.</param>
        /// <param name="maxPages">The maximum number of pages to keep in the cache.</param>
        public SqlCacheManager(string fileName, int maxPages)
        {
            _fileName = fileName;
            _maxPagesInCache = maxPages;
            _thresholdPages = (98 * _maxPagesInCache) / 100;
        }

        /// <summary>
        /// Retrieves a page.  Returns it from the buffer pool, or disk, as needed.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <returns>The page.</returns>
        public EnderSqlPage GetPage(uint pageNumber)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(pageNumber, out var sqlCacheEntry))
                {
                    sqlCacheEntry.LastRequested = DateTime.UtcNow;
                    return sqlCacheEntry.Page;
                }
            }


            //ok, so getting here means it wasn't in the cache, and we let the lock go            
            PruneCacheIfNeeded();  
            EnderSqlPage page = ReadPageFromDisk(pageNumber);
            lock (_cacheLock)
            {
                _cache.TryAdd(pageNumber, new SqlCacheEntry() { LastRequested = DateTime.UtcNow, Page = page });                
            }

            //Prefer asynchronous pruning to keep page count just under threshold.
            if (_cache.Count > _thresholdPages)
            {
                Task.Factory.StartNew(() => PruneCache(2));
            }
            return page;
        }

        /// <summary>
        /// Prune synchronously.
        /// </summary>
        private void PruneCacheIfNeeded()
        {
            if (_cache.Count >= _maxPagesInCache)
            {
                PruneCache(2);
            }            
        }

        /// <summary>
        /// Reads the given page from the disk.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        private EnderSqlPage ReadPageFromDisk(uint pageNumber)
        {
            lock (_diskLock)
            {
                byte[] buffer = new byte[65536];
                using (var fs = File.OpenRead(_fileName))
                {
                    fs.Seek(65536 * pageNumber, SeekOrigin.Begin);
                    var bytesRead = fs.Read(buffer, 0, 65536);
                    if (bytesRead != EnderSqlPage.PageLength)
                    {
                        throw new EnderSqlException();
                    }
                }
                EnderSqlPage page = new EnderSqlPage(buffer);
                page.IsDirty = false;
                return page;
            }
        }

        /// <summary>
        /// Flushes the page to disk.
        /// </summary>
        /// <param name="page"></param>
        public void FlushPageToDisk(EnderSqlPage page)
        {
            using (var fs = File.OpenWrite(_fileName))
            {                
                fs.Seek(65536 * page.PageNumber, SeekOrigin.Begin);
                fs.Write(page.PageData, 0, EnderSqlPage.PageLength);
                fs.Flush(true);
                page.IsDirty = false;                                
            }
        }

        /// <summary>
        /// Drops the entire cache.  Flushes all dirty pages to disk.  
        /// </summary>
        public void DropCache()
        {
            PruneCache(100);            
        }

        /// <summary>
        /// Removes the given percent of pages from the cache.  Flushes dirty pages to disk.  
        /// </summary>
        /// <remarks>
        /// Removes pages that haven't been requested recently.
        /// </remarks>
        /// <param name="percentToPurge"></param>
        public void PruneCache(int percentToPurge)
        {
            lock (_cacheLock)
            {
                int pagesToPurge = (percentToPurge * _cache.Count) / 100;
                var cacheList = _cache.ToArray().OrderBy(x=>x.Value.LastRequested).Take(pagesToPurge);
                foreach(var entry in cacheList)
                {
                    if (entry.Value.Page.IsDirty)
                    {
                        FlushPageToDisk(entry.Value.Page);
                    }
                    _cache.Remove(entry.Key);
                }
            }
        }

    }
}
