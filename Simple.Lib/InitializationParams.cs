using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
