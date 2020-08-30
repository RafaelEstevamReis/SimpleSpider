using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
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
        /// Gets the Target attribute of the tag
        /// </summary>
        public string Target => Attributes["target"];
    }
}
