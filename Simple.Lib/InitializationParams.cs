using System;
using System.IO;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider
{
    public class InitializationParams
    {
        public ICacher Cacher { get; set; }
        public IDownloader Downloader { get; set; }
        public DirectoryInfo SpiderDirectory { get; set; }
        public Configuration ConfigurationPrototype { get; set; }

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

        public InitializationParams SetConfig(Action<Configuration> Action)
        {
            Action(this.ConfigurationPrototype);
            return this; // Chaining
        }
    }
}
