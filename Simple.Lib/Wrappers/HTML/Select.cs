using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Select tag
    /// </summary>
    public class Select : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Select(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Select(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets all children Option tags
        /// </summary>
        public Option[] GetItems()
        {
            return GetChildren<Option>().ToArray();
        }
    }
}
