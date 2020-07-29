using System;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Represents a Link enqueued or fetched
    /// </summary>
    public class Link
    {

        /// <summary>
        /// Primary Uri, the resource to be fetched
        /// </summary>
        public Uri Uri { get; private set; }
        /// <summary>
        /// Uri where the Uri property was found
        /// </summary>
        public Uri SourceUri { get; }
        
        public Uri MovedUri { get; private set; }

        /// <summary>
        /// Fetch start Datetime
        /// </summary>
        public DateTime FetchStart { get; internal set; }
        /// <summary>
        /// Fetch finish Datetime
        /// </summary>
        public DateTime FetchEnd { get; internal set; }
        /// <summary>
        /// Fetch duration
        /// </summary>
        public TimeSpan FetchTime { get { return FetchEnd - FetchStart; } }
        
        /// <summary>
        /// Specify and additional CallBack for this resource
        /// </summary>
        public FetchComplete FetchCompleteCallBack { get; set; } = null;

        /// <summary>
        /// Constructs a new Link
        /// </summary>
        public Link(Uri Uri, Uri SourceUri)
        {
            this.MovedUri = null;
            this.Uri = Uri;
            this.SourceUri = SourceUri;
        }
        /// <summary>
        /// Returns the string representation of the Uri property
        /// </summary>
        public override string ToString()
        {
            return Uri.ToString();
        }
        /// <summary>
        /// Implicit convertion from Link to Uri, returns Uri property
        /// </summary>
        public static implicit operator Uri(Link lnk) => lnk.Uri;

        public void ResourceMoved(Uri newUri)
        {
            MovedUri = Uri;
            Uri = newUri;
        }
    }
}
