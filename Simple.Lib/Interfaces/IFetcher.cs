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

        void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config);

        void Start();
        void Stop();
    }
}
