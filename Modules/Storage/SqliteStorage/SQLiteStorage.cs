using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Storage
{
    /// <summary>
    /// Store values in a Sqlite database
    /// </summary>
    /// <typeparam name="T">Type of data to be stored</typeparam>
    public class SQLiteStorage<T> : IStorage
        where T : new()
    {
        List<(Link link, T item)> items;
        Timer tmrInsertBlock;
        object lockCommit;

        private readonly string tableNameT;
        private Sqlite.Database db;

        /// <summary>
        /// Create a new instance
        /// </summary>
        public SQLiteStorage()
        {
            tableNameT = typeof(T).Name;
            items = new List<(Link, T)>();
            lockCommit = new object();
            tmrInsertBlock = new Timer()
            {
                AutoReset = false, // must be re-enabled after commit
                Enabled = true,
                Interval = 5 * 1000, // 5s 
            };
            tmrInsertBlock.Elapsed += TmrInsertBlock_Elapsed;
        }
        /// <summary>
        /// Initialization method, will be called by the spider. Do not call it
        /// </summary>
        public void Initialize(Configuration Config)
        {
            db = new Sqlite.Database(System.IO.Path.Combine(Config.SpiderDataDirectory.FullName,
                                                            $"SqliteStorage_{tableNameT}.sqlite"));

            db.CreateTables()
              .Add<T>()
              .Add<ObjectReference>()
              .Commit();
        }

        /// <summary>
        /// LoadData, in Sqlite data is aways avalailable
        /// </summary>
        public void LoadData() { }
        /// <summary>
        /// Saves the data on Disk NOW
        /// </summary>
        public void SaveData(bool IsAutoSave)
        {
            commitQueue();
        }

        /// <summary>
        /// Adds and item the the database
        /// </summary>
        /// <param name="link">Link containing data about where the item was found</param>
        /// <param name="item">Item to be saved</param>
        /// <returns>For this storage, aways true</returns>
        public bool AddItem(Link link, dynamic item)
        {
            return AddItem(link, (T)item);
        }
        /// <summary>
        /// Adds and item the the database
        /// </summary>
        /// <param name="link">Link containing data about where the item was found</param>
        /// <param name="item">Item to be saved</param>
        /// <returns>For this storage, aways true</returns>
        public bool AddItem(Link link, T item)
        {
            enqueue((link, item));
            return true;
        }

        private void enqueue((Link link, T item) p)
        {
            items.Add(p);
        }

        private void TmrInsertBlock_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                commitQueue();
            }
            catch (Exception)
            {
                //this is still here for testing and debug
            }
        }

        private void commitQueue()
        {
            if (items.Count == 0) return;
            lock (lockCommit)
            {
                tmrInsertBlock.Enabled = false;

                var array = items.ToArray();
                items.Clear();

                var ids = db.BulkInsert(array.Select(a => a.item));

                var obs = new List<ObjectReference>();
                for (int i = 0; i < ids.Length; i++)
                {
                    obs.Add(new ObjectReference()
                    {
                        InsertedItem = ids[i],
                        CrawTime = DateTime.Now,
                        Uri = array[i].link
                    });
                }
                db.BulkInsert(obs);

                if (array.Length != ids.Length)
                {
                }
                if (array.Length != obs.Count)
                {
                }

                tmrInsertBlock.Enabled = true;
            }
        }

        /// <summary>
        /// Counts all records on the table, can be expensive on big tables
        /// </summary>
        /// <returns>How many records are on the table</returns>
        public int Count()
        {
            return db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableNameT}", null);
        }

        /// <summary>
        /// Returns all added items
        /// </summary>
        public IEnumerable<T> ReadAll()
        {
            // https://www.sqlite.org/rowidtable.html
            // > Access to records via rowid is highly optimized and very fast.

            return db.GetAll<T>();
        }
        /// <summary>
        /// Returns all added items with it's metadata
        /// </summary>
        public IEnumerable<(ObjectReference, T)> ReadAllReferenced()
        {
            // https://www.sqlite.org/rowidtable.html
            // > Access to records via rowid is highly optimized and very fast.
            // > BUT ... is not persistent and might change ... applications should not normally access the rowid directly ...

            var refs = db.GetAll<ObjectReference>();
            foreach (var r in refs)
            {
                var item = db.Get<T>("_rowid_", r.InsertedItem);

                yield return (r, item);
            }
        }

        /// <summary>
        /// Retrieve all stored items
        /// </summary>
        public IEnumerable<dynamic> RetrieveAllItems()
        {
            foreach (var i in ReadAll()) yield return i;
        }
    }
}
