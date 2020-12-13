using System.Linq;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Ordered List tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/ol">HTML element docs</a>
    /// </summary>
    public class Ol : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ol(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "ol"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ol(HtmlNode node) : base(node) { ThrowsIfNotName(node, "ol"); }

        /// <summary>
        /// Gets all children Li tags
        /// </summary>
        public Li[] GetItems()
        {
            return GetChildren<Li>().ToArray();
        }
    }
}