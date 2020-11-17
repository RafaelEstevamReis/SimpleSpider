using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Storage
{
    // Based on my https://github.com/RafaelEstevamReis/SqliteWrapper repository

    public class Database
    {
        private readonly object lockWrite;
        private readonly string cnnString;

        public string DatabaseFileName { get; }

        public Database(string fileName)
        {
            lockWrite = new object();
            DatabaseFileName = new FileInfo(fileName).FullName;
            // if now exists, creates one (can be done in the ConnectionString)
            if (!File.Exists(DatabaseFileName)) SQLiteConnection.CreateFile(DatabaseFileName);

            // uses builder to avoid escape issues
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder
            {
                DataSource = DatabaseFileName,
                Version = 3
            };
            cnnString = sb.ToString();
        }
        
        private SQLiteConnection getConnection()
        {
            var sqliteConnection = new SQLiteConnection(cnnString);
            sqliteConnection.Open();
            return sqliteConnection;
        }

        public TableMapper CreateTables()
        {
            return new TableMapper(this);
        }

        //public string[] GetAllTables()
        //{
        //    var dt = ExecuteReader(@"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", null);
        //    return dt.Rows.Cast<DataRow>()
        //                  .Select(row => (string)row[0])
        //                  .ToArray();
        //}
        public DataTable GetTableSchema(string TableName)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = $"SELECT * FROM {TableName} LIMIT 0";

            var reader = cmd.ExecuteReader();

            return reader.GetSchemaTable();
        }
        public int ExecuteNonReader(string Text, object Parameters = null)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            return cmd.ExecuteNonQuery();
        }

        private static void fillParameters(SQLiteCommand cmd, object Parameters)
        {
            foreach (var p in Parameters.GetType().GetProperties())
            {
                cmd.Parameters.AddWithValue(p.Name, p.GetValue(Parameters));
            }
        }
        private static IEnumerable<string> getNames(Type type, bool IncludeKey = true)
        {
            foreach (var info in type.GetProperties())
            {
                if (!IncludeKey)
                {
                    var keyAttrib = info.CustomAttributes
                                        .OfType<KeyAttribute>();
                    if (keyAttrib.Count() > 0) continue;
                }

                yield return info.Name;
            }
        }
    }
}
