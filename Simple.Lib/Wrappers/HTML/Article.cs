using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Article tag
    /// </summary>
    public class Article : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Article(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "article"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Article(HtmlNode node) : base(node) { ThrowsIfNotName(node, "article"); }
    }
}