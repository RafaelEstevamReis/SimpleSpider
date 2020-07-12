using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.RafaelEstevam.Spider.Cachers
{
    public class NullCacher : ICacher
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete) { }

        public bool HasCache(Uri uri) => false;
        public bool IsProcessing => false;

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config) { }

        public void Start() { }

        public void Stop() { }
    }
}
