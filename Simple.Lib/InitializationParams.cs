using System;
using System.Collections.Generic;
using System.IO;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Defines Initialization Parameters to a SimpleSpider instance
    /// </summary>
    public partial class InitializationParams
    {
        /// <summary>
        /// Creates a new InitializationParams
        /// </summary>
        public InitializationParams()
        {
            Parsers = new List<IParserBase>();
            ConfigurationPrototype = new Configuration();
        }

        /// <summary>
        /// Gets or sets the cacher to be used by the spider
        /// </summary>
        public ICacher Cacher { get; set; }
        /// <summary>
        /// Gets or sets the downloader to be used by the spider
        /// </summary>
        public IDownloader Downloader { get; set; }
        /// <summary>
        /// Gets the list of parsers the spider should be initialized with
        /// </summary>
        public List<IParserBase> Parsers { get; }
        /// <summary>
        /// Gets or sets the storage engine to be used by the spider
        /// </summary>
        public IStorage StorageEngine { get; set; }
        public IPageLinkCollector LinkCollector { get; set; }
        /// <summary>
        /// Gets or sets the working SpiderDirectory to be used by the spider
        /// </summary>
        public DirectoryInfo SpiderDirectory { get; set; }
        /// <summary>
        ///  Gets or sets a Configuration prototype to be used by the spider
        /// </summary>
        public Configuration ConfigurationPrototype { get; set; }

        /// <summary>
        /// Instructs spider to use this path
        /// </summary>
        public InitializationParams SetSpiderStartupPath(string Path)
        {
            if (string.IsNullOrEmpty(Path)) throw new ArgumentNullException(Path);
            var dir = new DirectoryInfo(Path);
            // Typo detection
            if (!dir.Exists) throw new DirectoryNotFoundException($"Path '{Path}' not found");

            SpiderDirectory = dir;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to use this directory
        /// </summary>
        public InitializationParams SetSpiderStartupDirectory(DirectoryInfo Directory)
        {
            SpiderDirectory = Directory;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to use this specific Cacher
        /// </summary>
        public InitializationParams SetCacher(ICacher cacher)
        {
            Cacher = cacher;
            return this; // Chaining 
        }
        /// <summary>
        /// Instructs spider to use this specific Downloader
        /// </summary>
        public InitializationParams SetDownloader(IDownloader downloader)
        {
            Downloader = downloader;
            return this; // Chaining
        }
        public InitializationParams SetLinkCollector(IPageLinkCollector linkCollector)
        {
            LinkCollector = linkCollector;
            return this; // Chaining
        }
        /// <summary>
        /// Instructs spider to OfflineMode, set the Downloader to a NullDownloader instance on 'Ignore Mode'
        /// </summary>
        public InitializationParams SetOfflineMode()
        {
            return SetDownloader(new Downloaders.NullDownloader());
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
        /// Instructs spider to use this specific Storage Engine
        /// </summary>
        public InitializationParams SetStorage(IStorage storage)
        {
            StorageEngine = storage;
            return this;
        }
        /// <summary>
        /// Sets a logger to be used by the spider instance
        /// </summary>
        public InitializationParams SetLogger(Serilog.ILogger logger)
        {
            ConfigurationPrototype.Logger = logger;
            return this;
        }
        /// <summary>
        /// Allows to a chainable Action to set some configurations
        /// </summary>
        public InitializationParams SetConfig(Action<Configuration> Action)
        {
            Action(ConfigurationPrototype);
            return this; // Chaining
        }

    }
}
