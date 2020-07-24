using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Helper;

namespace Net.RafaelEstevam.Spider
{
    public delegate void FetchComplete(object Sender, FetchCompleteEventArgs args);
    public delegate void FetchFail(object Sender, FetchFailEventArgs args);
    public delegate void FetchRewrite(object Sender, FetchRewriteEventArgs args);
    public delegate void ShouldFetch(object Sender, ShouldFetchEventArgs args);

    public class FetchEventArgs : EventArgs
    {
        public enum EventSource
        {
            Cacher,
            Downloader,
            Scheduler,
        }

        public HeaderCollection RequestHeaders { get; protected set; }
        public Link Link { get; protected set; }
        public EventSource Source { get; set; }
    }
    public class FetchCompleteEventArgs : FetchEventArgs
    {
        public byte[] Result { get; }
        public HeaderCollection ResponseHeaders { get; }

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
        public XElement GetXElement()
        {
            if (xElement == null)
            {
                xElement = HtmlToEXelement.Parse(Html);
            }
            return xElement;
        }

        public string HtmlContent(Encoding enc)
        {
            return htmlCache = enc.GetString(Result);
        }

        public FetchCompleteEventArgs(Link current, byte[] result, HeaderCollection requestHeaders, HeaderCollection responseHeaders)
        {
            this.Link = current;
            this.Result = result;
            this.RequestHeaders = requestHeaders;
            this.ResponseHeaders = responseHeaders;
        }
    }
    public class FetchFailEventArgs : FetchEventArgs
    {
        public Exception Error { get; }

        public FetchFailEventArgs(Link link, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.RequestHeaders = requestHeaders;
        }
    }

    public class ShouldFetchEventArgs : FetchEventArgs
    {
        public enum Reasons
        {
            AlreadyFetched,
            UserCancelled,
            UserCancelledSilent,
            None,
        }
        public bool Cancel { get; set; }
        public Reasons Reason { get; set; } = Reasons.None;

        public ShouldFetchEventArgs(Link link)
        {
            this.Link = link;
        }
    }
    public class FetchRewriteEventArgs : EventArgs
    {
        public FetchRewriteEventArgs(Uri CurrentUri)
        {
            this.CurrentUri = CurrentUri;
        }
        public Uri CurrentUri{ get; }
        public Uri NewUri { get; set; }
    }
}
