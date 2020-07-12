using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    public interface IFetcher
    {
        event FetchComplete FetchCompleted;
        event FetchFail FetchFailed;
        event ShouldFetch ShouldFetch;

        void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config);

        bool IsProcessing { get; }

        void Start();
        void Stop();
    }
}
