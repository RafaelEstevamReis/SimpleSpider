using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Net.RafaelEstevam.Spider
{
    public class Configuration
    {
        /// <summary>
        /// Time to wait in miliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; set; } = 5000;
        public DirectoryInfo SpiderDirectory { get; set; }
        public bool Cookies_Enable { get; set; }

        public bool Cache_Enable { get; set; } = true;
        public TimeSpan? Cache_Lifetime { get; set; }

    }
}
