using System;
using System.Text;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Helper;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Represents a method that passes fetch completed data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object containing fetch data</param>
    public delegate void FetchComplete(object Sender, FetchCompleteEventArgs args);
    /// <summary>
    /// Represents a method that passes fetch failed data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object containing fetch failed info</param>
    public delegate void FetchFail(object Sender, FetchFailEventArgs args);
    /// <summary>
    /// Represents a method that passes fetch rewrite data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object allowing changing the Uri</param>
    public delegate void FetchRewrite(object Sender, FetchRewriteEventArgs args);
    /// <summary>
    ///  Represents a method that checks if should fetch data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object allowing cancel the fetching process</param>
    public delegate void ShouldFetch(object Sender, ShouldFetchEventArgs args);

    /// <summary>
    /// Arguments to de Fetch event
    /// </summary>
    public class FetchEventArgs : EventArgs
    {
        /// <summary>
        /// Module that fired the event
        /// </summary>
        public enum EventSource
        {
            /// <summary>
            /// Event initiated by the Cacher
            /// </summary>
            Cacher,
            /// <summary>
            /// Event initiated by the Downloader
            /// </summary>
            Downloader,
            /// <summary>
            /// Event initiated by the Scheduler
            /// </summary>
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
    /// <summary>
    /// Arguments to de FetchComplete event
    /// </summary>
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
        /// <summary>
        /// Get the XElement representation of the Html property
        /// </summary>
        public XElement GetXElement()
        {
            if (xElement == null)
            {
                xElement = HtmlToXElement.Parse(Html);
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
        /// <summary>
        /// Constructs a new FetchCompleteEventArgs
        /// </summary>
        public FetchCompleteEventArgs(Link current, byte[] result, HeaderCollection requestHeaders, HeaderCollection responseHeaders)
        {
            this.Link = current;
            this.Result = result;
            this.RequestHeaders = requestHeaders;
            this.ResponseHeaders = responseHeaders;
        }
    }
    /// <summary>
    /// Arguments to de FetchFail event
    /// </summary>
    public class FetchFailEventArgs : FetchEventArgs
    {
        /// <summary>
        /// Error raised during fetch
        /// </summary>
        public Exception Error { get; }
        /// <summary>
        /// HTTP error code
        /// </summary>
        public int HttpErrorCode { get; }

        /// <summary>
        /// Constructs a FetchFailEventArgs
        /// </summary>
        [Obsolete]
        public FetchFailEventArgs(Link link, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.RequestHeaders = requestHeaders;
        }
        /// <summary>
        /// Constructs a FetchFailEventArgs
        /// </summary>
        public FetchFailEventArgs(Link link, int erroCode, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.HttpErrorCode = erroCode;
            this.RequestHeaders = requestHeaders;
        }
    }
    /// <summary>
    /// Arguments to de ShouldFetch event
    /// </summary>
    public class ShouldFetchEventArgs : FetchEventArgs
    {
        /// <summary>
        /// Reason to not fetch some resource
        /// </summary>
        public enum Reasons
        {
            /// <summary>
            /// This resource was already fetched
            /// </summary>
            AlreadyFetched,
            /// <summary>
            /// User cancelled the process
            /// </summary>
            UserCancelled,
            /// <summary>
            /// User cancelled the process, ignore on Log
            /// </summary>
            UserCancelledSilent,

            /// <summary>
            /// This resource caused an error on previous session
            /// </summary>
            PreviousError,

            /// <summary>
            /// There is no specific reason
            /// </summary>
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

        /// <summary>
        /// Creates a new ShouldFetchEventArgs
        /// </summary>
        public ShouldFetchEventArgs(Link link)
        {
            this.Link = link;
        }
    }
    /// <summary>
    /// Arguments to de FetchRewrite event
    /// </summary>
    public class FetchRewriteEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a FetchRewriteEventArgs
        /// </summary>
        /// <param name="CurrentUri"></param>
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
        /// <summary>
        /// Defines if should log the rewrite operation
        /// </summary>
        public bool ShowOnLog { get; set; } = true;
    }
}
