using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Paragraph tag
    /// </summary>
    public class Paragraph : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Paragraph(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "p"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Paragraph(HtmlNode node) : base(node) { ThrowsIfNotName(node, "p"); }
    }
}