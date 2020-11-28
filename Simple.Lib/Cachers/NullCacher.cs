using System;
using System.Collections.Concurrent;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Cachers
{
    /// <summary>
    /// Empty cacher, discards all cache requests
    /// </summary>
    public class NullCacher : ICacher
    {
#pragma warning disable 67
        /// <summary>
        /// Unused on this fetcher
        /// </summary>
        public event FetchComplete FetchCompleted;
        /// <summary>
        /// Unused on this fetcher
        /// </summary>
        public event FetchFail FetchFailed;
        /// <summary>
        /// Unused on this fetcher
        /// </summary>
        public event ShouldFetch ShouldFetch;
#pragma warning restore 67

        /// <summary>
        /// Instructs the cacher to generate a cache for the new resource
        /// </summary>
        /// <param name="FetchComplete">Fetch data to save</param>
        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete) { }
        /// <summary>
        /// Gets if a cache exists, always False for this cacher
        /// </summary>
        /// <param name="uri">Uri to check for</param>
        /// <returns>always false for this cacher</returns>
        public bool HasCache(Uri uri) => false;
        /// <summary>
        /// Gets if is processing, always False for this cacher
        /// </summary>
        public bool IsProcessing => false;
        /// <summary>
        /// Initialize the cacher
        /// </summary>
        /// <param name="WorkQueue">The queue to be used</param>
        /// <param name="Config">The configuration to be used</param>
        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config) { }
        /// <summary>
        /// Stats this cacher
        /// </summary>
        public void Start() { }
        /// <summary>
        /// Stops this cacher
        /// </summary>
        public void Stop() { }
    }
}
