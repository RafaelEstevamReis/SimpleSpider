using System;
using System.Collections.Generic;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Exposes extensions methods for Anchor
    /// </summary>
    public static class AnchorExtensions
    {
        /// <summary>
        /// Add an Anchor to fetch
        /// </summary>
        /// <param name="spider">Spider to add</param>
        /// <param name="anchor">Anchor to fetch</param>
        /// <param name="SourcePage">Uri where the Anchor was found</param>
        /// <returns>Link object</returns>
        public static Spider.Link AddPage(this SimpleSpider spider, Anchor anchor, Uri SourcePage)
        {
            return spider.AddPage(anchor.GetUri(SourcePage), SourcePage);
        }
        /// <summary>
        /// Adds an Anchors to fetch
        /// </summary>
        /// <param name="spider">Spider to add</param>
        /// <param name="anchors">Anchors to fetch</param>
        /// <param name="SourcePage">Uri where the Anchor was found</param>
        /// <returns>Array of Links</returns>
        public static Spider.Link[] AddPages(this SimpleSpider spider, IEnumerable<Anchor> anchors, Uri SourcePage)
        {
            return anchors.Select(a => AddPage(spider, a, SourcePage)).ToArray();
        }
    }
}