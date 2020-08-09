using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Represents a Link enqueued or fetched
    /// </summary>
    public class Link : IEquatable<Link>, IEquatable<Uri>
    {
        /// <summary>
        /// Primary Uri, the resource to be fetched
        /// </summary>
        public Uri Uri { get; private set; }
        /// <summary>
        /// Uri where the Uri property was found
        /// </summary>
        public Uri SourceUri { get; private set; }
        /// <summary>
        /// When redirected, the old Uri will be stored here
        /// </summary>
        public Uri MovedUri { get; private set; }
        /// <summary>
        /// When rewritten, the old Uri will be stored here
        /// </summary>
        public Uri RewrittenUri { get; private set; }

        /// <summary>
        /// Fetch start Datetime
        /// </summary>
        public DateTime FetchStart { get; set; }
        /// <summary>
        /// Fetch finish Datetime
        /// </summary>
        public DateTime FetchEnd { get; set; }
        /// <summary>
        /// Fetch duration
        /// </summary>
        public TimeSpan FetchTime { get { return FetchEnd - FetchStart; } }

        /// <summary>
        /// Specify and additional CallBack for this resource. Can not be saved/Loaded
        /// </summary>
        public FetchComplete FetchCompleteCallBack { get; set; } = null;

        private Link() { } // Empty, not safe

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
        /// Change the properties to reflect a moved resource
        /// </summary>
        public void ResourceMoved(Uri newUri)
        {
            MovedUri = Uri;
            Uri = newUri;
        }
        /// <summary>
        /// Change the properties to reflect a rewritten resource
        /// </summary>
        public void ResourceRewritten(Uri newUri)
        {
            RewrittenUri = Uri;
            Uri = newUri;
        }

        /// <summary>
        /// Implicit convertion from Link to Uri, returns Uri property
        /// </summary>
        public static implicit operator Uri(Link lnk) => lnk.Uri;
        /// <summary>
        /// Implicit convertion from Link to Url, returns Uri property
        /// </summary>
        public static implicit operator string(Link lnk) => lnk.Uri.ToString();

        /// <summary>
        /// Saves a link in a line-based http-like format
        /// </summary>
        /// <param name="link">Link to be saved</param>
        /// <returns>Lines</returns>
        public static IEnumerable<string> SaveLink(Link link)
        {
            yield return $"Uri: {link.Uri}";
            yield return $"SourceUri: {link.SourceUri}";
            if (link.MovedUri != null) yield return $"MovedUri: {link.MovedUri}";
            if (link.RewrittenUri != null) yield return $"RewrittenUri: {link.RewrittenUri}";

            yield return $"FetchStart: {link.FetchStart:yyyy-MM-ddTHH:mm:sszzz}";
            yield return $"FetchEnd: {link.FetchEnd:yyyy-MM-ddTHH:mm:sszzz}";
        }
        /// <summary>
        /// Loads a link from a line-based http-like format
        /// </summary>
        /// <param name="content">Lines to be saved</param>
        /// <returns>Link</returns>
        public static Link LoadLink(IEnumerable<string> content)
        {
            var lnk = new Link();

            foreach (var line in content)
            {
                int colIdx = line.IndexOf(':');
                if (colIdx <= 0) continue;

                string key = line.Substring(0, colIdx).ToLower();
                string value = line.Substring(colIdx + 1).TrimStart();

                switch (key)
                {
                    case "uri":
                        lnk.Uri = new Uri(value);
                        break;
                    case "sourceuri":
                        lnk.SourceUri = new Uri(value);
                        break;

                    case "moveduri":
                        lnk.MovedUri = new Uri(value);
                        break;
                    case "rewrittenuri":
                        lnk.RewrittenUri = new Uri(value);
                        break;

                    case "fetchstart":
                        lnk.FetchStart = DateTime.Parse(value);
                        break;
                    case "fetchend":
                        lnk.FetchEnd = DateTime.Parse(value);
                        break;
                }
            }

            if (lnk.Uri == null) throw new InvalidOperationException("Uri is not present");
            if (lnk.SourceUri == null) throw new InvalidOperationException("SourceUri is not present");

            return lnk;
        }

        /// <summary>
        /// Checks if two objects are equal
        /// </summary>
        /// <param name="obj">Second object to check</param>
        /// <returns>True if equals, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is Link link) return Equals(link);
            if (obj is Uri uri) return Equals(uri);
            return false;
        }

        /// <summary>
        /// Checks a Uri is equal to this Link, only Uri property is checked
        /// </summary>
        /// <param name="other">Second object to check</param>
        /// <returns>True if equals, false otherwise</returns>
        public bool Equals([AllowNull] Uri other)
        {
            if (other == null) return false;
            return Uri.Equals(other);
        }
        /// <summary>
        /// Checks if two objects are equal
        /// </summary>
        /// <param name="other">Second object to check</param>
        /// <returns>True if equals, false otherwise</returns>
        public bool Equals([AllowNull] Link other)
        {
            if (other == null) return false;

            if (Uri != other.Uri) return false;
            if (SourceUri != other.SourceUri) return false;
            if (MovedUri != other.MovedUri) return false;
            if (RewrittenUri != other.RewrittenUri) return false;
            if (FetchStart != other.FetchStart) return false;
            if (FetchEnd != other.FetchEnd) return false;
            return true;
        }
        /// <summary>
        /// Serves as the default hash function 
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
