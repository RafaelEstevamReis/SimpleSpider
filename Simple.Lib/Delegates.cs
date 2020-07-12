using Net.RafaelEstevam.Spider.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider
{
    public delegate void FetchComplete(object Sender, FetchCompleteEventArgs args);
    public delegate void FetchFail(object Sender, FetchFailEventArgs args);
    public delegate void ShouldFetch(object Sender, ShouldFetchEventArgs args);

    public class FetchEventArgs : EventArgs
    {
        public enum EventSource
        {
            Cacher,
            Downloader,
            Scheduler,
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
                if (htmlCache == null)
                {
                    var enc = Encoding.UTF8;
                    // check ResponseHeaders for encoding
                    return HtmlContent(enc);
                }
                return htmlCache;
            }
        }
        XElement xElement;
        public XElement XElement
        {
            get
            {
                if (xElement == null)
                {
                    xElement = HtmlToEXelement.Parse(Html);
                }
                return xElement;
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

    public class ShouldFetchEventArgs : FetchEventArgs
    {
        public enum Reasons
        {
            AlreadyFetched,
            UserCancelled,
            None,
        }
        public bool Cancel { get; set; }
        public Reasons Reason { get; set; } = Reasons.None;

        public ShouldFetchEventArgs(Link link)
        {
            this.Link = link;
        }
    }
}
