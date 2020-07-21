using System;
using System.Collections.Generic;
using System.IO;
using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider
{
    public class InitializationParams
    {
        public InitializationParams()
        {
            Parsers = new List<IParserBase>();
            ConfigurationPrototype = new Configuration();
        }

        public ICacher Cacher { get; set; }
        public IDownloader Downloader { get; set; }
        public List<IParserBase> Parsers { get; }
        public DirectoryInfo SpiderDirectory { get; set; }
        public Configuration ConfigurationPrototype { get; set; }

        public InitializationParams SetSpiderStarupPath(string Path)
        {
            if (string.IsNullOrEmpty(Path)) throw new ArgumentNullException(Path);
            var dir = new DirectoryInfo(Path);
            // Typo detection
            if (!dir.Exists) throw new DirectoryNotFoundException($"Path '{Path}' not found");

            this.SpiderDirectory = dir;
            return this; // Chaining
        }
        public InitializationParams SetSpiderStarupDirectory(DirectoryInfo Directory)
        {
            this.SpiderDirectory = Directory;
            return this; // Chaining
        }
        public InitializationParams SetCacher(ICacher Cacher)
        {
            this.Cacher = Cacher;
            return this; // Chaining
        }
        public InitializationParams SetDownloader(IDownloader Downloader)
        {
            this.Downloader = Downloader;
            return this; // Chaining
        }
        public InitializationParams SetOfflineMode()
        {
            return SetDownloader(new NullDownloader());
        }
        public InitializationParams AddParser<T>(IParser<T> Parser)
        {
            Parsers.Add(Parser);
            return this; // Chaining
        }
        public InitializationParams SetConfig(Action<Configuration> Action)
        {
            Action(this.ConfigurationPrototype);
            return this; // Chaining
        }

        /// <summary>
        /// Fronzen in time default: ContentCacher, WebClientDownloader, NoLimitCaching, and AutoAnchorsLinks enabled
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        /// <returns></returns>
        public static InitializationParams Default001(int DownloadDelay = 5000)
        {
            // Have non-chaging defaults helps with not breaking stuff
            //but still have a good start point

            return new InitializationParams()
                // Set stable fetchers, future change in defaults
                //will not affect this template
                .SetCacher(new ContentCacher()) // more stable for the time (the only one, but still)
                .SetDownloader(new WebClientDownloader())
                .SetConfig(c => c.Enable_Caching()
                                 .Disable_Cookies()
                                 .Set_CachingNoLimit()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Enable_AutoAnchorsLinks());
        }
    }
}
