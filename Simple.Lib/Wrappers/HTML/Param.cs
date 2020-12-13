using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Param tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/param">HTML element docs</a>
    /// </summary>
    public class Param : Tag, ITagValue, ITagName
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Param(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "param"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Param(HtmlNode node) : base(node) { ThrowsIfNotName(node, "param"); }

        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Value attribute of the tag
        /// </summary>
        public string Value => Attributes["value"];
    }
}
