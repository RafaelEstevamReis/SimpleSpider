using System;
using System.Collections.Generic;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Interfaces
{
    public interface IPageLinkCollector
    {
        bool ExecuteFallBackIfError { get; }
        bool CanProcessPage(FetchCompleteEventArgs args);

        IEnumerable<Uri> GetLinks(FetchCompleteEventArgs args);
        IPageLinkCollector FallBackProcessor { get; }

    }
}
