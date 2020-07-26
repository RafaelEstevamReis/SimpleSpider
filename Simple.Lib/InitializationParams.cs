using System;
using System.Collections.Generic;
using System.IO;
using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;
using Net.RafaelEstevam.Spider.Interfaces;
using Serilog;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Defines Initialization Parameters to a SimpleSpider instance
    /// </summary>
    public partial class InitializationParams
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

        /// <summary>
        /// Instructs spider to use this path
        /// </summary>
        public InitializationParams SetSpiderStarupPath(string Path)
        {
            if (string.IsNullOrEmpty(Path)) throw new ArgumentNullException(Path);
            var dir = new DirectoryInfo(Path);
            // Typo detection
            if (!dir.Exists) throw new DirectoryNotFoundException($"Path '{Path}' not found");

            this.SpiderDirectory = dir;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to use this directory
        /// </summary>
        public InitializationParams SetSpiderStarupDirectory(DirectoryInfo Directory)
        {
            this.SpiderDirectory = Directory;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to use this specific Cacher
        /// </summary>
        public InitializationParams SetCacher(ICacher Cacher)
        {
            this.Cacher = Cacher;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to use this specific Downloader
        /// </summary>
        public InitializationParams SetDownloader(IDownloader Downloader)
        {
            this.Downloader = Downloader;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to OfflineMode, set the Downloader to a NullDownloader instance on 'Ignore Mode'
        /// </summary>
        public InitializationParams SetOfflineMode()
        {
            return SetDownloader(new NullDownloader());
        }
        /// <summary>
        /// Adds a Parser to the spider. Parsers can be added and removed on-the-fly
        /// </summary>
        public InitializationParams AddParser<T>(IParser<T> Parser)
        {
            Parsers.Add(Parser);
            return this; // Chaining
        }
        /// <summary>
        /// Sets a logger to be used by the spider instance
        /// </summary>
        public InitializationParams SetLogger(ILogger logger)
        {
            ConfigurationPrototype.Logger = logger;
            return this;
        }
        /// <summary>
        /// Allows to a chainable Action to set some configurations
        /// </summary>
        public InitializationParams SetConfig(Action<Configuration> Action)
        {
            Action(this.ConfigurationPrototype);
            return this; // Chaining
        }

    }
}
