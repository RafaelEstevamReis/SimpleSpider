using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider
{
    public delegate void FetchComplete(object Sender, FetchCompleteEventArgs args);
    public delegate void FetchFail(object Sender, FetchFailEventArgs args);

    public class FetchEventArgs : EventArgs
    {
        public enum EventSource
        {
            Cacher,
            Downloader,
        }

        public Link Link { get; set; }
        public EventSource Source { get; set; }
    }
    public class FetchCompleteEventArgs : FetchEventArgs
    {
    }
    public class FetchFailEventArgs : FetchEventArgs
    {
    }
}
