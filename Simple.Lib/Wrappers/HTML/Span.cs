using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Span tag
    /// </summary>
    public class Span : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Span(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "span"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Span(HtmlNode node) : base(node) { ThrowsIfNotName(node, "span"); }
    }
}