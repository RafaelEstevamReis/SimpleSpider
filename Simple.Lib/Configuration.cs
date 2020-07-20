using System;

namespace Net.RafaelEstevam.Spider
{
    public class Configuration
    {
        /// <summary>
        /// Time to wait in miliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; set; } = 5000;
        /// <summary>
        /// Spider main directory
        /// </summary>
        public System.IO.DirectoryInfo SpiderDirectory { get; internal set; }
        /// <summary>
        /// Spider data directory
        /// </summary>
        public System.IO.DirectoryInfo SpiderDataDirectory { get; internal set; }
        /// <summary>
        /// Spider log file
        /// </summary>
        public string Spider_LogFile { get; internal set; }
        /// <summary>
        /// Standard log
        /// </summary>
        public Serilog.ILogger Logger { get; internal set; }
        /// <summary>
        /// Are cookies enabled? IDownloader must support
        /// </summary>
        public bool Cookies_Enable { get; set; }
        /// <summary>
        /// Are caching enabled? ICacher must support
        /// </summary>
        public bool Cache_Enable { get; set; } = true;
        /// <summary>
        /// How long cache files are valid?  ICacher must support
        /// </summary>
        public TimeSpan? Cache_Lifetime { get; set; }
        /// <summary>
        /// Enable auto colelction of html Anchors tags
        /// </summary>
        public bool Auto_AnchorsLinks { get; set; } = true;
    }
}
