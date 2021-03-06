﻿using System.Collections.Generic;
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
        int blockSize = 4 * 1024; // 4k
        /// <summary>
        /// Encoding to be used on read
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// Delimiter character
        /// </summary>
        public char Delimiter { get; set; } = ';';
        /// <summary>
        /// Defines if quoted fields can contain line-breaks
        /// </summary>
        public bool SupportQuotedLineBreaks { get; set; } = true;

        /// <summary>
        /// Read CSV data from a data stream
        /// </summary>
        /// <param name="stream">Base stream to read from</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public IEnumerable<string[]> ReadDelimiter(Stream stream)
        {
            StreamReader sr = new StreamReader(stream, Encoding);
            return ReadDelimiter(sr);
        }
        /// <summary>
        /// Read CSV data from a text stream
        /// </summary>
        /// <param name="stream">Base stream to read from</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public IEnumerable<string[]> ReadDelimiter(StreamReader stream)
        {
            List<string> columns = new List<string>();
            char[] buffer = new char[blockSize];
            int len;
            bool quoted = false;
            StringBuilder currentField = new StringBuilder();

            while ((len = stream.ReadBlock(buffer, 0, blockSize)) > 0)
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
                        // not double quote in the middle of the field ?
                        // not allowed
                        if (!quoted && currentField.Length > 0) continue;

                        quoted = !quoted;
                        continue;
                    }
                    else if (buffer[i] == Delimiter && !quoted)
                    {
                        columns.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else if (buffer[i] == '\n')
                    {
                        if (!SupportQuotedLineBreaks || !quoted)
                        {
                            columns.Add(currentField.ToString());
                            currentField.Clear();
                            yield return columns.ToArray();
                            columns.Clear();
                        }
                        else
                        {
                            currentField.Append(buffer[i]);
                        }
                    }
                    else if (buffer[i] == '\r')
                    {
                        if (!SupportQuotedLineBreaks || !quoted)
                        {
                            //ignore
                        }
                        else
                        {
                            currentField.Append(buffer[i]);
                        }
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

        /// <summary>
        /// Read CSV data from a data stream
        /// </summary>
        /// <param name="stream">Base stream to read from</param>
        /// <param name="encoding">Encoding to be used</param>
        /// <param name="delimiter">Delimiter to be used</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public static IEnumerable<string[]> ReadDelimiter(Stream stream, Encoding encoding = null, char delimiter = ';')
        {
            FastCsv fast = new FastCsv();
            if (encoding != null) fast.Encoding = encoding;
            fast.Delimiter = delimiter;
            return fast.ReadDelimiter(stream);
        }
        /// <summary>
        /// Read CSV data from a text stream
        /// </summary>
        /// <param name="stream">Base stream to read from</param>
        /// <param name="delimiter">Delimiter to be used</param>
        /// <returns>Enumeration of rows, Each row is a string[] of columns</returns>
        public static IEnumerable<string[]> ReadDelimiter(StreamReader stream, char delimiter = ';')
        {
            FastCsv fast = new FastCsv();
            fast.Delimiter = delimiter;
            return fast.ReadDelimiter(stream);
        }
    }
}
