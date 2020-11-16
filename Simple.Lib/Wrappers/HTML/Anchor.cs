using HtmlAgilityPack;
using RafaelEstevam.Simple.Spider.Helper;
using System;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Anchor tag
    /// </summary>
    public class Anchor : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Anchor(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "a"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Anchor(HtmlNode node) : base(node) { ThrowsIfNotName(node, "a"); }
        /// <summary>
        /// Gets the Href attribute of the tag
        /// </summary>
        public string Href => Attributes["href"];
        /// <summary>
        /// Combines ParentUri with Href and returns a clean Uri
        /// </summary>
        /// <param name="ParentUri">Parent Uri to combine to</param>
        /// <returns>An Uri combined without whitespaces</returns>
        public Uri GetUri(Uri ParentUri)
        {
            return UriHelper.Combine(ParentUri, Href, true);
        }

        /// <summary>
        /// Gets the Target attribute of the tag
        /// </summary>
        public string Target => Attributes["target"];
    }
}
