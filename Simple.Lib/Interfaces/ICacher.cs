using System;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    /// <summary>
    /// Represents a module to fetch resources from the disk/memory
    /// </summary>
    public interface ICacher : IFetcher
    {
        /// <summary>
        /// Returns if this module has a cache for this resource
        /// </summary>
        /// <param name="uri">Resource the be checked for</param>
        /// <returns>True if has a cache for the resource</returns>
        bool HasCache(Uri uri);
        /// <summary>
        /// Create cache for this fetched resource
        /// </summary>
        /// <param name="FetchComplete">The fetched resource to create a cache for</param>
        void GenerateCacheFor(FetchCompleteEventArgs FetchComplete);
    }
}
