using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    public interface ICacher : IFetcher
    {
        bool HasCache(Uri uri);
        void GenerateCacheFor(FetchCompleteEventArgs FetchComplete);
    }
}
