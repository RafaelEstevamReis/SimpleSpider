using System.IO;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Class to parse html with HtmlAgilityPack
    /// </summary>
    public static class HtmlParseHelper
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

        /// <summary>
        /// Parses an stream into a HtmlDocument
        /// </summary>
        /// <param name="stream">stream content</param>
        /// <returns>Html Document</returns>
        public static HtmlDocument ParseHtmlDocument(Stream stream)
        {
            // Static configs
            HtmlNode.ElementsFlags.Remove("form");

            HtmlDocument doc = new HtmlDocument
            {
                OptionOutputAsXml = true,
                OptionFixNestedTags = true,
                OptionReadEncoding = true
            };
            doc.Load(stream);
            return doc;
        }

        /// <summary>
        /// Decodes an HTML text using System.Net.WebUtility.HtmlDecode
        /// </summary>
        public static string HtmlDecode(string Encoded)
        {
            return System.Net.WebUtility.HtmlDecode(Encoded);
        }
    }
}
