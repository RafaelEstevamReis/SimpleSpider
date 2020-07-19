using System;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    public interface ICacher : IFetcher
    {
        bool HasCache(Uri uri);
        void GenerateCacheFor(FetchCompleteEventArgs FetchComplete);
    }
}
