using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Storage.Sqlite
{
    /// <summary>
    /// Class to map tables from a Types
    /// </summary>
    public partial class TableMapper
    {
        private readonly Database db;
        private readonly List<Table> tables;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public TableMapper(Database database)
        {
            db = database;
            tables = new List<Table>();
        }
        /// <summary>
        /// Adds a table
        /// </summary>
        public TableMapper Add<T>() where T : new()
        {
            tables.Add(Table.FromType(typeof(T)));
            return this;
        }
        /// <summary>
        /// Allows last added table to be editted
        /// </summary>
        public TableMapper ConfigureTable(Action<Table> Options)
        {
            Options(tables[^1]);
            return this;
        }
        /// <summary>
        /// Commit all new tables to the db (old schemas are not updated (yet)
        /// </summary>
        public void Commit()
        {
            foreach (var t in tables)
            {
                db.ExecuteNonQuery(t.ExportCreateTable(), null);
            }

            tables.Clear();
        }
        /// <summary>
        /// Represents a table schema
        /// </summary>
        public class Table
        {
            /// <summary>
            /// Table's name
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// Table's columns
            /// </summary>
            public Column[] Columns { get; set; }
            
            /// <summary>
            /// Creates a CREATE TABLE statment from current schema
            /// </summary>
            public string ExportCreateTable()
            {
                if (string.IsNullOrEmpty(TableName)) throw new ArgumentNullException("TableName can not be null");
                if (TableName.Any(c => char.IsWhiteSpace(c))) throw new ArgumentNullException("TableName can not contain whitespaces");
                if (TableName.Any(c => char.IsSymbol(c))) throw new ArgumentNullException("TableName can not contain symbols");

                if (Columns == null) throw new ArgumentNullException("Columns can not be null");
                if (Columns.Length == 0) throw new ArgumentNullException("Columns can not empty");

                /*
                 CREATE TABLE [IF NOT EXISTS] [schema_name].table_name (
                     column_1 data_type PRIMARY KEY,
                     column_2 data_type NOT NULL,
                     column_3 data_type DEFAULT 0,
                     table_constraints
                 ) [WITHOUT ROWID];
                 */

                StringBuilder sb = new StringBuilder();

                sb.Append("CREATE TABLE IF NOT EXISTS ");
                sb.Append(TableName);
                sb.Append("(\n");

                var columns = string.Join(',', Columns.Select(c => c.ExportColumnAsStatement()));
                sb.Append(columns);
                sb.Append("\n);");

                return sb.ToString();
            }
            
            /// <summary>
            /// Creates a table schema from a Type
            /// </summary>
            public static Table FromType(Type t)
            {
                var props = t.GetProperties();

                return new Table()
                {
                    TableName = t.Name,
                    Columns = props.Select(pi => Column.FromType(t, pi))
                                   .ToArray(),
                };
            }
        }
        /// <summary>
        /// Represents a column schema
        /// </summary>
        public class Column
        {
            /// <summary>
            /// Column name
            /// </summary>
            public string ColumnName { get; set; }
            /// <summary>
            /// Type on SQLite database
            /// </summary>
            public SqliteType SqliteType { get; set; }
            /// <summary>
            /// Native object type
            /// </summary>
            public Type NativeType { get; set; }
            /// <summary>
            /// Is PrimaryKey ?
            /// </summary>
            public bool IsPK { get; set; }
            /// <summary>
            /// Is Auto-Increment ?
            /// </summary>
            public bool IsAI { get; set; }
            /// <summary>
            /// Allow null values ?
            /// </summary>
            public bool AllowNulls { get; set; }
            /// <summary>
            /// Default value on NULL
            /// </summary>
            public object DefaultValue { get; set; }
           
            /// <summary>
            /// Creates a column schema from a Type
            /// </summary>
            public static Column FromType(Type t, PropertyInfo info)
            {

                SqliteType dataType;

                // Texts
                if (info.PropertyType == typeof(string)) dataType = SqliteType.TEXT;
                else if (info.PropertyType == typeof(Uri)) dataType = SqliteType.TEXT;
                // Float point Numbers
                else if (info.PropertyType == typeof(float)) dataType = SqliteType.REAL;
                else if (info.PropertyType == typeof(double)) dataType = SqliteType.REAL;
                // Fixed FloatPoint
                else if (info.PropertyType == typeof(decimal)) dataType = SqliteType.NUMERIC;
                // Integers
                else if (info.PropertyType == typeof(int)) dataType = SqliteType.INTEGER;
                else if (info.PropertyType == typeof(byte)) dataType = SqliteType.INTEGER;
                else if (info.PropertyType == typeof(long)) dataType = SqliteType.INTEGER;
                // Others Mapped of NUMERIC
                else if (info.PropertyType == typeof(bool)) dataType = SqliteType.NUMERIC;
                else if (info.PropertyType == typeof(DateTime)) dataType = SqliteType.NUMERIC;
                // Other
                else if (info.PropertyType == typeof(byte[])) dataType = SqliteType.BLOB;
                else
                {
                    throw new Exception($"Type {info.PropertyType.Name} is not supported on field {info.Name}");
                }
                bool isKey = IsKeyProp(info);

                return new Column()
                {
                    ColumnName = info.Name,
                    AllowNulls = dataType == SqliteType.TEXT
                                 || dataType == SqliteType.BLOB,
                    NativeType = info.PropertyType,
                    SqliteType = dataType,
                    DefaultValue = null,
                    IsPK = isKey,
                    IsAI = isKey && dataType == SqliteType.INTEGER
                };
            }

            internal static bool IsKeyProp(PropertyInfo info)
            {
                return info.GetCustomAttributes(typeof(KeyAttribute), true)
                           .FirstOrDefault() != null;
            }
            /// <summary>
            /// Creates a CREATE TABLE column statment from current schema
            /// </summary>
            public string ExportColumnAsStatement()
            {
                {
                    if (string.IsNullOrEmpty(ColumnName)) throw new ArgumentNullException("ColumnName can not be null");
                    if (ColumnName.Any(c => char.IsWhiteSpace(c))) throw new ArgumentNullException("ColumnName can not contain whitespaces");
                    if (ColumnName.Any(c => char.IsSymbol(c))) throw new ArgumentNullException("ColumnName can not contain symbols");

                    StringBuilder sb = new StringBuilder();

                    sb.Append(ColumnName);
                    sb.Append(" ");

                    sb.Append(SqliteType.ToString());
                    sb.Append(" ");

                    if (IsPK) sb.Append("PRIMARY KEY ");
                    if (IsAI) sb.Append("AUTOINCREMENT ");

                    if (!AllowNulls) sb.Append("NOT NULL ");

                    if (DefaultValue != null)
                    {
                        sb.Append($"DEFAULT '{DefaultValue}'");
                    }

                    return sb.ToString();
                }
            }

        }
    }
}
