namespace RafaelEstevam.Simple.Spider.Storage.Sqlite
{
    public partial class TableMapper
    {
        /// <summary>
        /// Sqlite types
        /// </summary>
        public enum SqliteType
        {
            /// <summary>
            /// 
            /// </summary>
            INTEGER,
            /// <summary>
            /// The value is a text string, stored using the database encoding (UTF-8, UTF-16BE or UTF-16LE)
            /// </summary>
            TEXT,
            /// <summary>
            /// The value is a blob of data, stored exactly as it was input
            /// </summary>
            BLOB,
            /// <summary>
            /// The value is a floating point value, stored as an 8-byte IEEE floating point number
            /// </summary>
            REAL,
            /// <summary>
            /// For Decimal, Bool, Date, DateTime
            /// </summary>
            NUMERIC,
        }
    }
}
