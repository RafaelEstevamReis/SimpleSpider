using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Div tag
    /// </summary>
    public class Div : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Div(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "div"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Div(HtmlNode node) : base(node) { ThrowsIfNotName(node, "div"); }
    }
}