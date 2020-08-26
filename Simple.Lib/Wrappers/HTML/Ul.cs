using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Unordered List tag
    /// </summary>
    public class Ul : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ul(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Ul(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets all children Li tags
        /// </summary>
        public Li[] GetItems()
        {
            return GetChildren<Li>().ToArray();
        }
    }
}