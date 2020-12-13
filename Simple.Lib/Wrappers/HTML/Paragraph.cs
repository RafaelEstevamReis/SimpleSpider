using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Paragraph tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/p">HTML element docs</a>
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