using System;

namespace Net.RafaelEstevam.Spider
{
    public class Configuration
    {
        #region Internals SET
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

        #endregion

        #region Cache Stuff

        /// <summary>
        /// Are caching enabled? ICacher must support
        /// </summary>
        public bool Cache_Enable { get; set; } = true;

        public Configuration Enable_Caching()
        {
            Cache_Enable = true;
            return this; // Chaining
        }
        public Configuration Disable_Caching()
        {
            Cache_Enable = false;
            return this; // Chaining
        }

        /// <summary>
        /// How long cache files are valid?  ICacher must support
        /// </summary>
        public TimeSpan? Cache_Lifetime { get; set; }

        public Configuration Set_CachingTTL(TimeSpan timeSpan)
        {
            Cache_Lifetime = timeSpan;
            return this; // Chaining
        }
        public Configuration Set_CachingNoLimit()
        {
            Cache_Lifetime = null;
            return this; // Chaining
        }

        #endregion

        #region Downloader Stuff

        /// <summary>
        /// Time to wait in miliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; set; } = 5000;
        public Configuration Set_DownloadDelay(TimeSpan Delay)
        {
            DownloadDelay = (int)Delay.TotalMilliseconds;
            return this; // Chaining
        }
        public Configuration Set_DownloadDelay(int Delay)
        {
            DownloadDelay = Delay;
            return this; // Chaining
        }

        /// <summary>
        /// Are cookies enabled? IDownloader must support
        /// </summary>
        public bool Cookies_Enable { get; set; }
        public Configuration Enable_Cookies()
        {
            Cookies_Enable = true;
            return this; // Chaining
        }
        public Configuration Disable_Cookies()
        {
            Cookies_Enable = false;
            return this; // Chaining
        }

        #endregion

        #region Spider Pós-processing

        /// <summary>
        /// Enable auto colelction of html Anchors tags
        /// </summary>
        public bool Auto_AnchorsLinks { get; set; } = true;

        public Configuration Enable_AutoAnchorsLinks()
        {
            Auto_AnchorsLinks = true;
            return this; // Chaining
        }
        public Configuration Disable_AutoAnchorsLinks()
        {
            Auto_AnchorsLinks = false;
            return this; // Chaining
        }

        #endregion
    }
}
