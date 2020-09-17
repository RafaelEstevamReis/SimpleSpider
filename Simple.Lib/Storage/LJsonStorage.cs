using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Storage
{
    /// <summary>
    /// LineJson-based storage engine
    /// </summary>
    public class LJsonStorage : IStorage
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
            filePath = Path.Combine(config.SpiderDataDirectory.FullName, "results.ljson");
            stream = new StreamWriter(filePath, true);

            config.Logger.Information("LineJson Storage: " + filePath);
        }
        /// <summary>
        /// Adds a new item
        /// </summary>
        /// <param name="link">Link where the object was found</param>
        /// <param name="item">Item to be collected</param>
        /// <returns>Returns true always</returns>
        public bool AddItem(Link link, dynamic item)
        {
            var line = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.None);
            stream.WriteLine(line);
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
            if (!IsAutoSave) stream.Dispose();
        }
    }
}
