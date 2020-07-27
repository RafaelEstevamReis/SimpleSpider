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

        /// <summary>
        /// The request headers used to query
        /// </summary>
        public HeaderCollection RequestHeaders { get; protected set; }
        /// <summary>
        /// Link from where the resource was fetched
        /// </summary>
        public Link Link { get; protected set; }
        /// <summary>
        /// Source from this event
        /// </summary>
        public EventSource Source { get; internal set; }
    }
    public class FetchCompleteEventArgs : FetchEventArgs
    {
        /// <summary>
        /// Byte array with the data fetched
        /// </summary>
        public byte[] Result { get; }
        /// <summary>
        /// The response headers returned 
        /// </summary>
        public HeaderCollection ResponseHeaders { get; }

        // Lazy loaded html string
        string htmlCache;
        /// <summary>
        /// LazyLoaded Html (string) content parsed from  byte[] Result encoded with UTF8
        /// </summary>
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

        // Lazy loaded xElement
        XElement xElement;
        public XElement GetXElement()
        {
            if (xElement == null)
            {
                xElement = HtmlToEXelement.Parse(Html);
            }
            return xElement;
        }
        /// <summary>
        /// Parses  byte[] Result using and specific Encoding. The 'Html' property will be updated with this value
        /// </summary>
        /// <param name="enc">Encoding to be used</param>
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
        /// <summary>
        /// Error raised during fetch
        /// </summary>
        public Exception Error { get; }
        public int HttpErrorCode { get; }

        public FetchFailEventArgs(Link link, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.RequestHeaders = requestHeaders;
        }
        public FetchFailEventArgs(Link link, int erroCode, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.HttpErrorCode = erroCode;
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

            PreviousError,

            None,
        }
        /// <summary>
        /// Instruct the spider to NOT fetch this resource
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// Informs reason to do not fetch
        /// </summary>
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
        /// <summary>
        /// The original Uri added to the queue
        /// </summary>
        public Uri CurrentUri{ get; }
        /// <summary>
        /// New Uri to be fetched
        /// </summary>
        public Uri NewUri { get; set; }
    }
}
