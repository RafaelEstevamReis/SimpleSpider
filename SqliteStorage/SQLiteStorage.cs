using RafaelEstevam.Simple.Spider.Interfaces;
using System;

namespace RafaelEstevam.Simple.Spider.Storage
{
    public class SQLiteStorage<T> : IStorage
        where T : new()
    {
        private readonly string tableNameT;
        private Sqlite.Database db;

        public SQLiteStorage()
        {
            tableNameT = typeof(T).Name;
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
        // There is no need to save
        public void SaveData(bool IsAutoSave) { }

        public bool AddItem(Link link, dynamic item)
        {
            return AddItem(link, (T)item);
        }
        public bool AddItem(Link link, T item)
        {
            // I'll not store the Link YET
            var id = db.Insert(item);
            db.Insert(new ObjectReference()
            {
                CrawTime = DateTime.Now,
                Uri = link.Uri,
                InsertedItem = id,
            });

            return true;
        }

        public int Count()
        {
            return db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableNameT}", null);
        }
    }
}
