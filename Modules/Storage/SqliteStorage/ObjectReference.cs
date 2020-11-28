using System;

namespace RafaelEstevam.Simple.Spider.Storage
{
    /// <summary>
    /// Intermediate class to storage item metadata
    /// </summary>
    public class ObjectReference
    {
        /// <summary>
        /// Crawl Uri 
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// Crawl timestamp
        /// </summary>
        public DateTime CrawTime { get; set; }
        /// <summary>
        /// ID on the Item table
        /// </summary>
        public long InsertedItem { get; set; }
    }
}
