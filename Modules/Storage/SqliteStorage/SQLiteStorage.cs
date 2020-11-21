using RafaelEstevam.Simple.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace RafaelEstevam.Simple.Spider.Storage
{
    public class SQLiteStorage<T> : IStorage
        where T : new()
    {
        List<(Link link, T item)> items;
        Timer tmrInsertBlock;
        object lockCommit;

        private readonly string tableNameT;
        private Sqlite.Database db;

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
        public void Initialize(Configuration Config)
        {
            db = new Sqlite.Database(System.IO.Path.Combine(Config.SpiderDataDirectory.FullName,
                                                            $"SqliteStorage_{tableNameT}.sqlite"));

            db.CreateTables()
              .Add<T>()
              .Add<ObjectReference>()
              .Commit();
        }

        // There is no need to load
        public void LoadData() { }
        public void SaveData(bool IsAutoSave)
        {
            commitQueue();
        }

        public bool AddItem(Link link, dynamic item)
        {
            return AddItem(link, (T)item);
        }
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
            commitQueue();
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

        public int Count()
        {
            return db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableNameT}", null);
        }
    }
}
