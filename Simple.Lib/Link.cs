using System;

namespace Net.RafaelEstevam.Spider
{
    public class Link
    {
        public Uri Uri { get; }
        public Uri SourceUri { get; }
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
