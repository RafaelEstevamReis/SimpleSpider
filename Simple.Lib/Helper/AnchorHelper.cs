﻿using System;
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
                // Checa quotem só no começo
                if (sHref.Substring(0, 6).Contains('"')) quote = '"';
                else if (sHref.Substring(0, 6).Contains('\'')) quote = '\'';
                else
                {
                    // no quote:  `href=/` 
                    // <a href=https://twitter.com/nome_doSite rel=noopener >
                    continue;
                }

                sHref = sHref.Substring(sHref.IndexOf(quote) + 1);
                sHref = sHref.Substring(0, sHref.IndexOf(quote));

                if (sHref.StartsWith("javascript:")) continue;
                var uri = request.Combine(sHref);
                if (uri == null) continue;

                yield return uri;
            }
        }

        /// <summary>
        /// Get all anchors ('a' tag) and convert to an Uri collection
        /// </summary>
        public static IEnumerable<Uri> GetAnchors(Uri request, HtmlDocument root)
        {
            var hRefValues = root.DocumentNode
                .SelectNodes(".//a")
                .Select(x => x.Attributes["href"])
                .Where(att => att != null)
                .Where(at => !at.Value.Contains("javascript:"))
                //.Where(at => at.Value.Length > 10 || !(at.Value.StartsWith("http://") || at.Value.StartsWith("https://")))
                .Select(at => at.Value);

            return tentaCriarUri(request, hRefValues);
        }
        private static IEnumerable<Uri> tentaCriarUri(Uri request, IEnumerable<string> hRefValues)
        {
            foreach (var val in hRefValues)
            {
                if (Uri.TryCreate(request, val, out Uri uri))
                {
                    yield return uri;
                }
                else
                {
                    // invalid uri
                }
            }
        }
    }
}
