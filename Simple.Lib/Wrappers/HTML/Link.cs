using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Link tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/link">HTML element docs</a>
    /// </summary>
    public class Link : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Link(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "link"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Link(HtmlNode node) : base(node) { ThrowsIfNotName(node, "link"); }

        /// <summary>
        /// Gets the Href attribute of the tag
        /// </summary>
        public string Href => Attributes["href"];
        /// <summary>
        /// Gets the Rel attribute of the tag
        /// </summary>
        public string Rel => Attributes["rel"];
    }
}
