using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.RafaelEstevam.Spider.Cachers
{
    /// <summary>
    /// Empty cacher, discards all cache requests
    /// </summary>
    public class NullCacher : ICacher
    {
#pragma warning disable 67
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;
#pragma warning restore 67

        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete) { }

        public bool HasCache(Uri uri) => false;
        public bool IsProcessing => false;

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config) { }

        public void Start() { }

        public void Stop() { }
    }
}
