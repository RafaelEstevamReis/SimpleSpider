using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Simple.UnitTests")]
namespace RafaelEstevam.Simple.Spider.Helper
{    
    /// <summary>
     /// Helper to read huge csv files
     /// </summary>
    public class FastCsv
    {
        static int blockSize = 4 * 1024; // 4k
        /// <summary>
        /// Read CSV data from a data stream
        /// </summary>
        /// <param name="stream">Base stream to read from</param>
        /// <param name="encoding">Encoding to be used</param>
        /// <param name="delimiter">Delimiter to be used</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public static IEnumerable<string[]> ReadDelimiter(Stream stream, Encoding encoding = null, char delimiter = ';')
        {
            encoding ??= Encoding.UTF8;

            using StreamReader sr = new StreamReader(stream, encoding);
            return ReadDelimiter(sr, delimiter);
        }
        /// <summary>
        /// Read CSV data from a text stream
        /// </summary>
        /// <param name="sr">Base stream to read from</param>
        /// <param name="delimiter">Delimiter to be used</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public static IEnumerable<string[]> ReadDelimiter(StreamReader sr, char delimiter = ';')
        {
            List<string> columns = new List<string>();
            char[] buffer = new char[blockSize];
            int len;
            bool quoted = false;
            StringBuilder currentField = new StringBuilder();

            while ((len = sr.ReadBlock(buffer, 0, blockSize)) > 0)
            {
                for (int i = 0; i < len; i++)
                {
                    if (buffer[i] == '"')
                    {
                        // double-quotes
                        if (quoted && i < len - 1 && buffer[i + 1] == '"')
                        {
                            currentField.Append(buffer[i]);
                            i++; // ignore this
                            continue; // ignore next
                        }

                        quoted = !quoted;
                        continue;
                    }
                    else if (buffer[i] == delimiter && !quoted)
                    {
                        columns.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else if (buffer[i] == '\n' && !quoted)
                    {
                        columns.Add(currentField.ToString());
                        currentField.Clear();
                        yield return columns.ToArray();
                        columns.Clear();
                    }
                    else if (buffer[i] == '\r' && !quoted)
                    {
                        // ignore
                    }
                    else
                    {
                        currentField.Append(buffer[i]);
                    }
                }
            }

            columns.Add(currentField.ToString());
            var end = columns.ToArray();

            if (end.Length == 1 && end[0].Length == 0) yield break;

            yield return end;
        }
    }
}
