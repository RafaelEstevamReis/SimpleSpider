using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Helper
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
            int idx;
            int offSet = 0;
            char quote = '"';
            while ((idx = htmlContent.IndexOf("<a ", offSet)) >= 0)
            {
                offSet = idx + 1; // Advance

                int end = htmlContent.IndexOf('>', idx);
                if (end < 0) yield break; // Html tag does not end?

                int href = htmlContent.IndexOf("href", idx);
                if (href < 0) yield break; // there is none href left in html

                if (href > end) continue; //this <a don't have href

                string sHref = htmlContent[href..end];

                if (sHref.Contains('"')) quote = '"';
                else if (sHref.Contains('\'')) quote = '\'';

                sHref = sHref.Substring(sHref.IndexOf(quote) + 1);
                sHref = sHref.Substring(0, sHref.IndexOf(quote));

                if (sHref.StartsWith("javascript:")) continue;
                yield return request.Combine(sHref);
            }
        }

        /// <summary>
        /// Get all anchors ('a' tag) and convert to an Uri collection
        /// </summary>
        public static IEnumerable<Uri> GetAnchors(Uri request, HtmlDocument root)
        {
            return root.DocumentNode
                .SelectNodes(".//a")
                .Select(x => x.Attributes["href"])
                .Where(att => att != null)
                .Where(at => !at.Value.Contains("javascript:"))
                .Select(at => new Uri(request, at.Value));
        }
    }
}
