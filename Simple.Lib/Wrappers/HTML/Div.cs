using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Div tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/div">HTML element docs</a>
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