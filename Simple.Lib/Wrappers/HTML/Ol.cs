using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Ordered List tag
    /// </summary>
    public class Ol : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ol(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ol(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets all children Li tags
        /// </summary>
        public Li[] GetItems()
        {
            return GetChildren<Li>().ToArray();
        }
    }
}