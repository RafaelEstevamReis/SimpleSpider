using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RafaelEstevam.Simple.Spider.LinkProcessors
{
    public class HtmlDocumentAnchorProcessor : IPageLinkCollector
    {
        public IPageLinkCollector FallBackProcessor => new SimpleProcessor();
        public bool ExecuteFallBackIfError => true;

        public bool CanProcessPage(Uri request, string htmlContent) 
            => htmlContent.Length > 30 && htmlContent.Substring(0, 20).Contains("html", StringComparison.InvariantCultureIgnoreCase);

        public IEnumerable<Uri> GetLinks(Uri request, string htmlContent)
        {
            var document = HtmlParseHelper.ParseHtmlDocument(htmlContent);
            return AnchorHelper.GetAnchors(request, document);
        }
    }
}
