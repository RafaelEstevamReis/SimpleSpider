using System;

namespace Net.RafaelEstevam.Spider
{
    public class Link
    {
        public Uri Uri { get; }
        public Uri SourceUri { get; }

        public DateTime FetchStart { get; internal set; }
        public DateTime FetchEnd { get; internal set; }
        public TimeSpan FetchTime { get { return FetchEnd - FetchStart; } }

        public Link(Uri Uri, Uri SourceUri)
        {
            this.Uri = Uri;
            this.SourceUri = SourceUri;
        }
        public override string ToString()
        {
            return Uri.ToString();
        }

        public static implicit operator Uri(Link lnk) => lnk.Uri;
    }
}
