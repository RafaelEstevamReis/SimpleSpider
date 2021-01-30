using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Helper
{
    public class FastCsv
    {
        static int blockSize = 1024;

        public static IEnumerable<string[]> ReadCsv(Stream stream, Encoding encoding, char delimiter)
        {
            List<string> columns = new List<string>();
            using StreamReader sr = new StreamReader(stream, encoding);

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
            currentField.Clear();
            yield return columns.ToArray();

        }
    }
}
