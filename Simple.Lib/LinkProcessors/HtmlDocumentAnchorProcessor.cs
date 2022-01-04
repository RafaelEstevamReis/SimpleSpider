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

        public bool CanProcessPage(FetchCompleteEventArgs args) 
            => args.Html.Length > 30 && args.Html.Substring(0, 20).Contains("html", StringComparison.InvariantCultureIgnoreCase);
        
        public IEnumerable<Uri> GetLinks(FetchCompleteEventArgs args)
        {
            return AnchorHelper.GetAnchors(args.Link, args.GetDocument());
        }
    }
}
