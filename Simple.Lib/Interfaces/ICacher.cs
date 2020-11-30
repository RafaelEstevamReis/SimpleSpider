using System;

namespace RafaelEstevam.Simple.Spider.Interfaces
{
    /// <summary>
    /// Represents a module to fetch resources from the disk/memory
    /// </summary>
    public interface ICacher : IFetcher
    {
        /// <summary>
        /// Occurs before fetch to check if the cache can be used
        /// </summary>
        event ShouldUseCache ShouldUseCache;

        /// <summary>
        /// Returns if this module has a cache for this resource
        /// </summary>
        /// <param name="link">Resource the be checked for</param>
        /// <returns>True if has a cache for the resource</returns>
        bool HasCache(Link link);
        /// <summary>
        /// Create cache for this fetched resource
        /// </summary>
        /// <param name="FetchComplete">The fetched resource to create a cache for</param>
        void GenerateCacheFor(FetchCompleteEventArgs FetchComplete);
    }
}
