using System;
using System.Collections.Generic;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Interfaces
{
    public interface IPageLinkCollector
    {
        bool ExecuteFallBackIfError { get; }
        bool CanProcessPage(Uri request, string htmlContent);
        IEnumerable<Uri> GetLinks(Uri request, string htmlContent);
        IPageLinkCollector FallBackProcessor { get; }
    }
}
