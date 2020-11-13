using System;

namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Represents a method that passes fetch failed data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object containing fetch failed info</param>
    public delegate void FetchFail(object Sender, FetchFailEventArgs args);

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
        public FetchFailEventArgs(Link link, int errorCode, Exception error, HeaderCollection requestHeaders)
        {
            this.Link = link;
            this.Error = error;
            this.HttpErrorCode = errorCode;
            this.RequestHeaders = requestHeaders;
        }
    }
}