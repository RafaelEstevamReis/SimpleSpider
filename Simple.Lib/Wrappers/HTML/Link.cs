using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Link tag
    /// </summary>
    public class Link : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Link(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Link(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Href attribute of the tag
        /// </summary>
        public string Href => Attributes["href"];
        /// <summary>
        /// Gets the Rel attribute of the tag
        /// </summary>
        public string Rel => Attributes["rel"];
    }
}
