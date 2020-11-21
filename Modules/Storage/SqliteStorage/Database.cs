﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Storage.Sqlite
{
    // Based on my https://github.com/RafaelEstevamReis/SqliteWrapper repository

    public class Database
    {
        // Manual lock on Writes to avoid Exceptions
        private readonly object lockNonQuery;
        private readonly string cnnString;

        public string DatabaseFileName { get; }

        public Database(string fileName)
        {
            lockNonQuery = new object();
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

        public string[] GetAllTables()
        {
            var dt = ExecuteReader(@"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", null);
            return dt.Rows.Cast<DataRow>()
                          .Select(row => (string)row[0])
                          .ToArray();
        }
        public DataTable GetTableSchema(string TableName)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = $"SELECT * FROM {TableName} LIMIT 0";

            var reader = cmd.ExecuteReader();

            return reader.GetSchemaTable();
        }
        public int ExecuteNonQuery(string Text, object Parameters = null)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            lock (lockNonQuery)
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string Text, object Parameters)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            var obj = cmd.ExecuteScalar();

            // In SQLite DateTime is returned as STRING after aggregate operations
            if (typeof(T) == typeof(DateTime))
            {
                if (DateTime.TryParse(obj.ToString(), out DateTime dt))
                {
                    return (T)(object)dt;
                }
                return default;
            }

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public DataTable ExecuteReader(string Text, object Parameters)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            DataTable dt = new DataTable();
            var da = new SQLiteDataAdapter(cmd.CommandText, cnn);
            da.Fill(dt);
            return dt;
        }

        public IEnumerable<T> ExecuteQuery<T>(string Text, object Parameters)
            where T : new()
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                var schema = reader.GetSchemaTable();
                var colNames = schema.Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["ColumnName"])
                    .ToHashSet();

                while (reader.Read())
                {
                    // build new
                    T t = new T();

                    foreach (var p in typeof(T).GetProperties())
                    {
                        if (!colNames.Contains(p.Name)) continue;

                        mapColumn(t, p, reader);
                    }
                    yield return t;
                }
            }
        }

        private static void mapColumn<T>(T obj, System.Reflection.PropertyInfo p, SQLiteDataReader reader)
            where T : new()
        {
            object objVal;

            if (p.PropertyType == typeof(string)) objVal = reader.GetValue(p.Name);
            else if (p.PropertyType == typeof(Uri)) objVal = new Uri((string)reader.GetValue(p.Name));
            else if (p.PropertyType == typeof(double)) objVal = reader.GetDouble(p.Name);
            else if (p.PropertyType == typeof(float)) objVal = reader.GetFloat(p.Name);
            else if (p.PropertyType == typeof(decimal)) objVal = reader.GetDecimal(p.Name);
            else if (p.PropertyType == typeof(int)) objVal = reader.GetInt32(p.Name);
            else if (p.PropertyType == typeof(long)) objVal = reader.GetInt64(p.Name);
            else if (p.PropertyType == typeof(bool)) objVal = reader.GetBoolean(p.Name);
            else if (p.PropertyType == typeof(DateTime)) objVal = reader.GetDateTime(p.Name);
            else if (p.PropertyType == typeof(byte[])) objVal = (byte[])reader.GetValue(p.Name);
            else objVal = reader.GetValue(p.Name);

            if (objVal is DBNull) objVal = null;

            p.SetValue(obj, objVal);
        }

        public T Get<T>(object KeyValue)
            where T : new()
        {
            var TypeT = typeof(T);

            string keyColumn = TypeT.GetProperties()
                                    .Where(p => p.GetCustomAttributes(true) 
                                                 .Any(a => a is KeyAttribute))
                                    .FirstOrDefault()
                                    ?.Name;            
            if (keyColumn == null) throw new ArgumentException("Type dows not define a Key Column");

            var tableName = TypeT.Name;

            return ExecuteQuery<T>($"SELECT * FROM {tableName} WHERE {keyColumn} = @KeyValue ", new { KeyValue })
                    .FirstOrDefault();
        }
        public IEnumerable<T> GetAll<T>()
            where T : new()
        {
            var tableName = typeof(T).Name;

            return ExecuteQuery<T>($"SELECT * FROM {tableName} ", null);
        }

        public long Insert<T>(T Item)
        {
            string sql = buildInsertSql<T>();
            // lock(lockWrite) // NonQuery already locks
            return ExecuteScalar<long>(sql, Item);
        }
        private static string buildInsertSql<T>()
        {
            var TypeT = typeof(T);
            var tableName = TypeT.Name;

            var names = getNames(TypeT, false); // Not the Keys
            var fields = string.Join(',', names);
            var values = string.Join(',', names.Select(n => $"@{n}"));

            return $"INSERT INTO {tableName} ({fields}) VALUES ({values}); SELECT last_insert_rowid();";
        }
        public void BulkInsert<T>(IEnumerable<T> Items)
        {
            string sql = buildInsertSql<T>();

            using var cnn = getConnection();

            lock (lockNonQuery)
            {
                using var trn = cnn.BeginTransaction();

                foreach (var item in Items)
                {
                    using var cmd = new SQLiteCommand(sql, cnn, trn);
                    fillParameters(cmd, item);
                    cmd.ExecuteNonQuery();
                }

                trn.Commit();
            }
        }

        private static void fillParameters(SQLiteCommand cmd, object Parameters)
        {
            if (Parameters == null) return;
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