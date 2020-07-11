using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
