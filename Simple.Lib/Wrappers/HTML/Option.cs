using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Option List tag
    /// </summary>
    public class Option : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Option(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Option(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Label attribute of the tag
        /// </summary>
        public string Label => Attributes["label"];
        /// <summary>
        /// Gets the Value attribute of the tag
        /// </summary>
        public string Value => Attributes["value"];

        /// <summary>
        /// Gets the Selected attribute of the tag
        /// </summary>
        public bool Selected => Attributes["selected"] != null;
        /// <summary>
        /// Gets the Disabled attribute of the tag
        /// </summary>
        public bool Disabled => Attributes["disabled"] != null;
    }
}
