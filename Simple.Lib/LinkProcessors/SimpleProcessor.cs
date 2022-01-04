using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Interfaces;
using System;
using System.Collections.Generic;

namespace RafaelEstevam.Simple.Spider.LinkProcessors
{
    public class SimpleProcessor : IPageLinkCollector
    {
        public IPageLinkCollector FallBackProcessor => null;
        public bool ExecuteFallBackIfError => false;

        public bool CanProcessPage(FetchCompleteEventArgs args) => true;

        public IEnumerable<Uri> GetLinks(FetchCompleteEventArgs args)
        {
            string htmlContent = args.Html;
            // AnchorHelper.GetAnchors(request, htmlContent);
            if (htmlContent.StartsWith("<?xml"))
            {
                // rss
                foreach (var link in htmlContent.Split("<link"))
                {
                    if (link == null) continue;
                    if (link.Length < 5) continue;
                    if (link[1] == '?') continue;

                    var content = link.Substring(link.IndexOf('>') + 1);
                    content = content.Substring(0, content.IndexOf('<'));

                    if (content.StartsWith("http")) yield return new Uri(content);
                }
            }
            else
            {
                foreach (var l in AnchorHelper.GetAnchors(args.Link, htmlContent)) yield return l;
            }
        }
    }
}
