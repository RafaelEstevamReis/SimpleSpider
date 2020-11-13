using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleToAttribute("Simple.UnitTests")]
namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Helper to do stuff with CSV data
    /// </summary>
    public static class CSVHelper
    {
        /// <summary>
        /// Split CSV lines using semicolon (rfc4180 standard)
        /// </summary>
        /// <param name="Lines">Lines to be splitted</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> CSVSplit(IEnumerable<string> Lines)
        {
            // rfc4180 standard
            return DelimiterSplit(Lines, ';');
        }
        /// <summary>
        /// Split CSV lines using specified delimiter
        /// </summary>
        /// <param name="Lines">Lines to be splitted</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> DelimiterSplit(IEnumerable<string> Lines, char delimiter)
        {
            foreach (var line in Lines)
            {
                yield return splitLine(line, delimiter).ToArray();
            }
        }
        internal static IEnumerable<string> splitLine(string line, char delimiter)
        {
            bool isQuoted = false;
            var sb = new StringBuilder();
            // I will not implement escape now
            foreach (var c in line)
            {
                if (c == '"')
                {
                    isQuoted = !isQuoted;
                    continue;
                }
                if (!isQuoted && c == delimiter)
                {
                    yield return sb.ToString();
                    sb.Clear();
                    continue;
                }

                sb.Append(c);
            }
            yield return sb.ToString();
        }


        /// <summary>
        /// Splits a CSV file even if its compressed as .gz or .zip
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="encoding">Specify which encoding should be used</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> FileSplit(string path, Encoding encoding = null, char delimiter = ';')
        {
            return FileSplit(new FileInfo(path), encoding, delimiter);
        }

        /// <summary>
        /// Splits a CSV file even if its compressed as .gz or .zip
        /// </summary>
        /// <param name="fi">File to be read</param>
        /// <param name="encoding">Specify which encoding should be used</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> FileSplit(FileInfo fi, Encoding encoding = null, char delimiter = ';')
        {
            if (encoding == null) encoding = Encoding.Default;

            using var fs = fi.OpenRead();

            IEnumerable<string[]> items;
            // choose the correct source
            if (fi.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                items = zippedFileSpit(fs, encoding, delimiter);
            else if (fi.Extension.Equals(".gz", StringComparison.InvariantCultureIgnoreCase))
                items = compressedFileSpit(fs, encoding, delimiter);
            else
                items = FileSplit(new StreamReader(fs, encoding), delimiter);
            // enumerate it
            foreach (var i in items) yield return i;
            // close after finished all items
            fs.Close();
        }

        /// <summary>
        /// Load DataRows from csv file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="encoding">Specify which encoding should be used</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <param name="hasHeader">Defines if the first row is the header</param>
        /// <returns></returns>
        public static IEnumerable<DataRow> LoadRows(string path, Encoding encoding = null, char delimiter = ';', bool hasHeader = false)
        {
            DataTable dt = null;
            foreach (var line in FileSplit(path, encoding, delimiter))
            {
                if (dt == null) // first
                {
                    dt = new DataTable();

                    for (int i = 0; i < line.Length; i++)
                    {
                        dt.Columns.Add();
                    }

                    if (hasHeader)
                    {
                        // skip this line
                        continue;
                    }
                }

                var row = dt.NewRow();
                row.ItemArray = line;
                yield return row;
            }
        }

        /// <summary>
        /// Read a CSV file even if its compressed as .gz or .zip
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="encoding">Specify which encoding should be used</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <param name="hasHeader">Defines if the first row is the header</param>
        /// <returns>A datatable with data</returns>
        public static DataTable LoadFile(string path, Encoding encoding = null, char delimiter = ';', bool hasHeader = false)
        {
            return ToDataTable(FileSplit(path, encoding, delimiter), hasHeader);
        }

        private static IEnumerable<string[]> compressedFileSpit(FileStream fs, Encoding encoding, char delimiter)
        {
            var gz = new GZipStream(fs,  CompressionMode.Decompress);
            foreach (var v in FileSplit(new StreamReader(gz, encoding), delimiter))
            {
                yield return v;
            }
        }

        private static IEnumerable<string[]> zippedFileSpit(FileStream fs, Encoding encoding, char delimiter)
        {
            var zip = new ZipArchive(fs, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                using var zipStream = entry.Open();
                foreach (var v in FileSplit(new StreamReader(zipStream, encoding), delimiter))
                {
                    yield return v;
                }
            }
        }
        /// <summary>
        /// Splits lines from a stream
        /// </summary>
        /// <param name="streamReader">Stream to get lines from</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> FileSplit(StreamReader streamReader, char delimiter = ';')
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                yield return splitLine(line, delimiter).ToArray();
            }
        }

        /// <summary>
        /// Exports the CSV result to a DataTable
        /// </summary>
        /// <param name="data">Data returned from other methods</param>
        /// <param name="hasHeader">Defines if the first row is the header</param>
        /// <returns>A datatable with data</returns>
        public static DataTable ToDataTable(this IEnumerable<string[]> data, bool hasHeader = false)
        {
            DataTable dt = new DataTable();

            var e = data.GetEnumerator(); // lets do it in steps ...

            if (hasHeader)
            {
                if (e.MoveNext())
                {
                    foreach (var c in e.Current)
                    {
                        dt.Columns.Add(c);
                    }
                }
            }
            while (e.MoveNext())
            {
                dt.Rows.Add(e.Current);
            }
            return dt;
        }

    }
}
