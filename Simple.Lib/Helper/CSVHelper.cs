using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleToAttribute("Simple.UnitTests")]
namespace Net.RafaelEstevam.Spider.Helper
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
        /// <param name="fi">File to be read</param>
        /// <param name="encoding">Specify which encoding should be used</param>
        /// <param name="delimiter">Specify which delimiter should be used</param>
        /// <returns>Enumeration of an array of strings</returns>
        public static IEnumerable<string[]> FileSplit(FileInfo fi, Encoding encoding = null, char delimiter = ';')
        {
            if (encoding == null) encoding = Encoding.Default;

            using (var fs = fi.OpenRead())
            {
                string[] compressed = { ".gz", ".zip" };
                if (compressed.Contains(fi.Extension))
                {
                    return compressedFileSpit(fs, encoding, delimiter);
                }
                else
                {
                    return FileSplit(new StreamReader(fs, encoding), delimiter);
                }
            }
        }

        private static IEnumerable<string[]> compressedFileSpit(FileStream fs, Encoding encoding, char delimiter)
        {
            var zip = new ZipArchive(fs, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries)
            {
                using (var zipStream = entry.Open())
                {
                    foreach (var v in FileSplit(new StreamReader(zipStream, encoding), delimiter))
                    {
                        yield return v;
                    }
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

    }
}
