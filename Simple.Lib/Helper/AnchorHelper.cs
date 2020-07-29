using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Helper to do stuff with html {a} tags 
    /// </summary>
    public static class AnchorHelper
    {
        /// <summary>
        /// Simple Substring based 'a' tag enumerator
        /// </summary>
        public static IEnumerable<Uri> GetAnchors(Uri request, string htmlContent)
        {
            var lstA = htmlContent.Split("<a ").Skip(1);
            string href;
            int idx;
            foreach (var a in lstA)
            {
                try
                {
                    idx = a.IndexOf("href");
                    if (idx < 0) continue;
                    href = a.Substring(idx);
                    href = href.Substring(href.IndexOf('"') + 1);
                    href = href.Substring(0, href.IndexOf('"'));

                    if (href.StartsWith("javascript:")) continue;
                }
                catch { continue; }

                yield return request.Combine(href);

                //var builder = new UriBuilder(request);
                //if (builder.Path.EndsWith("/") && href.StartsWith("/")) href = href.Substring(1);
                //builder.Path += href;
                //
                //yield return builder.Uri;
            }
        }
        /// <summary>
        /// Get all anchors ('a' tag) and convert to an Uri collection
        /// </summary>
        public static IEnumerable<Uri> GetAnchors(Uri request, XElement root)
        {
            return root
                .XPathSelectElements(".//a")
                .Select(x => x.Attribute("href"))
                .Where(att => att != null)
                .Where(at => !at.Value.Contains("javascript:"))
                .Select(at => new Uri(request, at.Value));
        }
    }
}
