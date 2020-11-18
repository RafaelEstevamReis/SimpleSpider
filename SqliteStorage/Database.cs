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
        public int ExecuteNonReader(string Text, object Parameters = null)
        {
            using var cnn = getConnection();
            using var cmd = cnn.CreateCommand();

            cmd.CommandText = Text;
            fillParameters(cmd, Parameters);

            return cmd.ExecuteNonQuery();
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

        public IEnumerable<T> ExecuteQuery<T>(string Text, object Parameters) where T : new()
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
                    .ToArray();

                while (reader.Read())
                {
                    // build new
                    T t = new T();

                    foreach (var p in typeof(T).GetProperties())
                    {
                        if (!colNames.Contains(p.Name)) continue;

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

                        p.SetValue(t, objVal);
                    }
                    yield return t;
                }
            }
        }

        public T Get<T>(object KeyValue) where T : new()
        {
            string keyColumn = null;
            //var keyColumn = 
            if (keyColumn == null) throw new ArgumentException("Type dows not define a Key Column");

            var tableName = typeof(T).Name;

            return ExecuteQuery<T>($"SELECT * FROM {tableName} WHERE {keyColumn} = @KeyValue ", new { KeyValue })
                    .FirstOrDefault();
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
