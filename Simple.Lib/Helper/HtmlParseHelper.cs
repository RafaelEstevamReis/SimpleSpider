using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    public class HtmlParseHelper
    {
        /// <summary>
        /// Parses an Html string into a HtmlDocument
        /// </summary>
        /// <param name="html">Html content</param>
        /// <returns>Html Document</returns>
        public static HtmlDocument ParseHtmlDocument(string html)
        {
            // Static configs
            HtmlNode.ElementsFlags.Remove("form");

            HtmlDocument doc = new HtmlDocument
            {
                OptionOutputAsXml = true,
                OptionFixNestedTags = true,
                OptionReadEncoding = true
            };
            doc.LoadHtml(html);
            return doc;
        }

    }
}
