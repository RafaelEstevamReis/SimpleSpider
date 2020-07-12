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
        public DirectoryInfo SpiderDirectory { get; internal set; }
    }
}
