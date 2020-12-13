using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html List Item tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/li">HTML element docs</a>
    /// </summary>
    public class Li : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Li(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "li"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Li(HtmlNode node) : base(node) { ThrowsIfNotName(node, "li"); }
    }
}
