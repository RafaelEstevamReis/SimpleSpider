using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Data tag
    /// </summary>
    public class Data : Tag, ITagValue
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Data(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "data"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Data(HtmlNode node) : base(node) { ThrowsIfNotName(node, "data"); }

        /// <summary>
        /// Gets the Value attribute of the tag
        /// </summary>
        public string Value => Attributes["value"];
    }
}
