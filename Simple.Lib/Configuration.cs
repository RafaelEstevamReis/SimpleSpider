using System;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Class to hold the configuration for the spider
    /// </summary>
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

        #region Scheduler
        /// <summary>
        /// Enable auto rewrite of Uri to remove Fragment
        /// </summary>
        public bool Auto_RewriteRemoveFragment { get; set; } = false;

        /// <summary>
        /// Enables automatic UriRewrite to remove fragments
        /// </summary>
        public Configuration Enable_AutoRewriteRemoveFragment()
        {
            Auto_RewriteRemoveFragment = true;
            return this; // Chaining
        }
        /// <summary>
        /// Disables automatic UriRewrite to remove fragments
        /// </summary>
        public Configuration Disable_AutoRewriteRemoveFragment()
        {
            Auto_RewriteRemoveFragment = false;
            return this; // Chaining
        }

        #endregion

        #region Cache Stuff

        /// <summary>
        /// Are caching enabled? ICacher must support
        /// </summary>
        public bool Cache_Enable { get; set; } = true;
        /// <summary>
        /// Enables Caching using Cache_Enable property
        /// </summary>
        public Configuration Enable_Caching()
        {
            Cache_Enable = true;
            return this; // Chaining
        }
        /// <summary>
        /// Disables Caching using Cache_Enable property
        /// </summary>
        public Configuration Disable_Caching()
        {
            Cache_Enable = false;
            return this; // Chaining
        }

        /// <summary>
        /// How long cache files are valid?  ICacher must support
        /// </summary>
        public TimeSpan? Cache_Lifetime { get; set; }
        /// <summary>
        /// Sets cache limit
        /// </summary>
        /// <param name="timeSpan">Max age of the cache resource</param>
        public Configuration Set_CachingTTL(TimeSpan timeSpan)
        {
            Cache_Lifetime = timeSpan;
            return this; // Chaining
        }
        /// <summary>
        /// Sets cache limit to Infinity (no limit), using Cache_Lifetime property set to null
        /// </summary>
        public Configuration Set_CachingNoLimit()
        {
            Cache_Lifetime = null;
            return this; // Chaining
        }

        #endregion

        #region Downloader Stuff

        /// <summary>
        /// Time to wait in milliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; set; } = 5000;
        /// <summary>
        /// Sets Downloader delay using DownloadDelay property
        /// </summary>
        /// <param name="Delay">Delay value</param>
        public Configuration Set_DownloadDelay(TimeSpan Delay)
        {
            DownloadDelay = (int)Delay.TotalMilliseconds;
            return this; // Chaining
        }
        /// <summary>
        /// Sets Downloader delay in milliseconds using DownloadDelay property
        /// </summary>
        /// <param name="Delay">Value in milliseconds</param>
        public Configuration Set_DownloadDelay(int Delay)
        {
            DownloadDelay = Delay;
            return this; // Chaining
        }

        /// <summary>
        /// Are cookies enabled? IDownloader must support
        /// </summary>
        public bool Cookies_Enable { get; set; } = false;
        /// <summary>
        /// Enables downloader to use Cookies, if supported. Uses Cookies_Enable property
        /// </summary>
        /// <returns></returns>
        public Configuration Enable_Cookies()
        {
            Cookies_Enable = true;
            return this; // Chaining
        }
        /// <summary>
        /// Disables downloader from using Cookies, if supported. Uses Cookies_Enable property
        /// </summary>
        /// <returns></returns>
        public Configuration Disable_Cookies()
        {
            Cookies_Enable = false;
            return this; // Chaining
        }

        #endregion

        #region Pause Downloader AND Cacher

        /// <summary>
        /// Inform all components to temporarily pause
        /// </summary>
        public bool Paused { get; set; } = false;
        /// <summary>
        /// Inform cacher to temporarily pause
        /// </summary>
        public bool Paused_Cacher { get; set; } = false;
        /// <summary>
        /// Inform downloader to temporarily pause
        /// </summary>
        public bool Paused_Downloader { get; set; } = false;

        #endregion

        #region Spider Pós-processing

        /// <summary>
        /// Enable auto collection of html Anchors tags
        /// </summary>
        public bool Auto_AnchorsLinks { get; set; } = true;
        /// <summary>
        /// Enable auto collection of html Anchors tags, uses Auto_AnchorsLinks property
        /// </summary>
        public Configuration Enable_AutoAnchorsLinks()
        {
            Auto_AnchorsLinks = true;
            return this; // Chaining
        }
        /// <summary>
        /// Disable auto collection of html Anchors tags, uses Auto_AnchorsLinks property
        /// </summary>
        /// <returns></returns>
        public Configuration Disable_AutoAnchorsLinks()
        {
            Auto_AnchorsLinks = false;
            return this; // Chaining
        }

        #endregion
    }
}
