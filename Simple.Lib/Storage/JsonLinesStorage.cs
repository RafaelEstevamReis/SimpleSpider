using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Storage
{
    /// <summary>
    /// JsonLines-based storage engine
    /// <a href="https://jsonlines.org/">See more</a>
    /// </summary>
    public class JsonLinesStorage : IStorage
    {
        private Configuration config;
        string filePath;
        StreamWriter stream;
        /// <summary>
        /// Initializes the storage
        /// </summary>
        public void Initialize(Configuration Config)
        {
            this.config = Config;
            filePath = Path.Combine(config.SpiderDataDirectory.FullName, "results.jsonl");

            if (File.Exists(filePath))
            {
                backupFile();
            }

            stream = new StreamWriter(filePath, true);

            config.Logger.Information("JsonLines Storage at " + filePath);
        }
        private void backupFile()
        {
            var bkp1 = Path.Combine(config.SpiderDataDirectory.FullName, "results.old.gz");
            var bkp2 = Path.Combine(config.SpiderDataDirectory.FullName, "results.older.gz");
            var tmp = Path.GetTempFileName();
            using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write))
            {
                using var gz = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Compress);
                using var fileToSave = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fileToSave.CopyTo(gz);
                gz.Flush();
                fs.Flush();
            }

            if (File.Exists(bkp1))
            {
                // netstandard2.1 File.Move don't have OverrideFile parameter
                if (File.Exists(bkp2)) File.Delete(bkp2);
                File.Move(bkp1, bkp2);
            }

            File.Move(tmp, bkp1);
        }

        /// <summary>
        /// Adds a new item
        /// </summary>
        /// <param name="link">Link where the object was found</param>
        /// <param name="item">Item to be collected</param>
        public bool AddItem(Link link, dynamic item)
        {
            if (stream == null)
            {
                config.Logger.Error(new Exception("Unable to store Item, the stream was closed"), $"Unable to store {link} data");
                return false;
            }

            var line = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.None);
            lock (filePath)
            {
                stream.WriteLine(line);
            }
            return true;
        }
        /// <summary>
        /// Not supported by this Storage
        /// </summary>
        /// <returns>Returns -1</returns>
        public int Count()
        {
            return -1;
        }
        /// <summary>
        /// Not supported by this storage, does nothing
        /// </summary>
        public void LoadData() { }
        /// <summary>
        /// Saves all data
        /// </summary>
        /// <param name="IsAutoSave">Indicates if is an Auto-save or if is closing. Call with True will dispose the stream</param>
        public void SaveData(bool IsAutoSave)
        {
            stream.Flush();
            if (!IsAutoSave)
            {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
