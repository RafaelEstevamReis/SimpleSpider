using System;

namespace RafaelEstevam.Simple.Spider.Events
{
    /// <summary>
    /// Represents a method that checks if should use the cache
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object allowing cancel the caching load process</param>
    public delegate void ShouldUseCache(object Sender, ShouldUseCacheEventArgs args);

    /// <summary>
    /// Arguments to de ShouldFetch event
    /// </summary>
    public class ShouldUseCacheEventArgs : FetchEventArgs
    {
        /// <summary>
        /// Gets when the cache was generated
        /// </summary>
        public DateTime PageCacheDate { get; set; }
        /// <summary>
        /// Gets the cache age
        /// </summary>
        public TimeSpan PageCacheAge => DateTime.Now - PageCacheDate;

        /// <summary>
        /// Instruct the spider to NOT use this cache
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Sets [Cancel] to True if Link.Uri contains [PartialUrl]
        /// </summary>
        /// <param name="PartialUrl">String to be searched</param>
        public ShouldUseCacheEventArgs CancelIfContains(string PartialUrl)
        {
            if (Link.Contains(PartialUrl)) Cancel = true;
            return this;
        }

        /// <summary>
        /// Sets [Cancel] to True if PageCacheAge is older than [span]
        /// </summary>
        public ShouldUseCacheEventArgs CancelIfOlderThan(TimeSpan span)
        {
            if (PageCacheAge > span) Cancel = true;
            return this;
        }
        /// <summary>
        /// Sets [Cancel] to True if PageCacheAge is newer than [span]
        /// </summary>
        public ShouldUseCacheEventArgs CancelIfNewerThan(TimeSpan span)
        {
            if (PageCacheAge < span) Cancel = true;
            return this;
        }

        /// <summary>
        /// Creates a new ShouldUseCacheEventArgs
        /// </summary>
        public ShouldUseCacheEventArgs(Link link)
        {
            this.Link = link;
        }
    }
}
