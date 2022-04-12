using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.LinkProcessors
{
    public class HtmlDocumentMultiTag : IPageLinkCollector
    {
        public IPageLinkCollector FallBackProcessor => new SimpleProcessor();
        public bool ExecuteFallBackIfError => true;

        public bool CanProcessPage(FetchCompleteEventArgs args)
            => args.Html.Length > 30 && args.Html.Substring(0, 20).Contains("html", StringComparison.InvariantCultureIgnoreCase);

        public IEnumerable<Uri> GetLinks(FetchCompleteEventArgs args)
        {
            var doc = args.GetDocument();
            // <a
            foreach (var uri in AnchorHelper.GetAnchors(args.Link, doc)) yield return uri;

            // <button onClick
            var buttons = doc.DocumentNode
               .SelectNodes(".//button")
               .Select(x => x.Attributes["onclick"])
               .Where(att => att != null);

            foreach (var btn in buttons)
            {
                string value = btn.Value;
                string url;
                try
                {
                    if (!value.Contains("window.open")) continue;

                    url = value.Substring(value.IndexOf("(") + 1);
                    url = url.Substring(0, url.IndexOf(")"));

                    int idxQuote = url.IndexOf("'");
                    int idxDoubleQuote = url.IndexOf("\"");

                    char delimiter = '"';
                    if (idxDoubleQuote < 0) delimiter = '\'';
                    if (idxQuote >= 0 && idxQuote < idxDoubleQuote) delimiter = '\'';

                    int idxStart = url.IndexOf(delimiter) + 1; // skip quote
                    int idxEnd = url.IndexOf(delimiter, idxStart + 1);

                    if (idxStart < 0) continue;
                    if (idxEnd < 0) continue;

                    url = url.Substring(idxStart, idxEnd - idxStart);

                }
                catch { continue; }

                yield return new Uri(args.Link, url);
            }

        }
    }
}
