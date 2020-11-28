using System.Linq;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Unordered List tag
    /// </summary>
    public class Ul : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ul(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "ul"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ul(HtmlNode node) : base(node) { ThrowsIfNotName(node, "ul"); }

        /// <summary>
        /// Gets all children Li tags
        /// </summary>
        public Li[] GetItems()
        {
            return GetChildren<Li>().ToArray();
        }
    }
}