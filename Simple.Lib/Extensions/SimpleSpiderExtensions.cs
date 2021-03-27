using System;
using System.Collections.Generic;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;

namespace RafaelEstevam.Simple.Spider
{
    public static class SimpleSpiderExtensions
    {
        public static void AddPages(this SimpleSpider spider, IEnumerable<Anchor> iA, Link source)
        {
            foreach (var a in iA) AddPage(spider, a, source);
        }
        public static void AddPage(this SimpleSpider spider, Anchor a, Link source)
        {
            var url = a.Href.Replace("\t", "")
                            .Replace("\r", "")
                            .Replace("\n", "")
                            .Replace("&amp;", "&");
            var uri = new Uri(source, url);
            spider.AddPage(uri, source);
        }
    }
}
