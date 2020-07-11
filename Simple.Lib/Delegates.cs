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
        public byte[] Result { get; }
        public KeyValuePair<string, string>[] ResponseHeaders { get; }

        string htmlCache;
        public string Html
        {
            get
            {
                if (htmlCache == null) return HtmlContent(Encoding.Default);
                return htmlCache;
            }
        }

        public string HtmlContent(Encoding enc)
        {
            return htmlCache = enc.GetString(Result);
        }

        public FetchCompleteEventArgs(Link current, byte[] result, KeyValuePair<string, string>[] responseHeaders)
        {
            this.Link = current;
            this.Result = result;
            this.ResponseHeaders = responseHeaders;
        }
    }
    public class FetchFailEventArgs : FetchEventArgs
    {
        public Exception Error { get; }

        public FetchFailEventArgs(Link link, Exception error)
        {
            this.Link = link;
            this.Error = error;
        }
    }
}
