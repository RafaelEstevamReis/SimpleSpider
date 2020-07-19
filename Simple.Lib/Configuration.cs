using System;
using System.IO;

namespace Net.RafaelEstevam.Spider
{
    public class Configuration
    {
        /// <summary>
        /// Time to wait in miliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; set; } = 5000;
        public DirectoryInfo SpiderDirectory { get; internal set; }
        public DirectoryInfo SpiderDataDirectory { get; internal set; }
        public string Spider_SaveCollectedFile { get; internal set; }
        public bool Cookies_Enable { get; set; }

        public bool Cache_Enable { get; set; } = true;
        public TimeSpan? Cache_Lifetime { get; set; }

    }
}
