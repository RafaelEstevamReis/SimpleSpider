using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Article tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/article">HTML element docs</a>
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