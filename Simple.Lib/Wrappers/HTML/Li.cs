﻿using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html List Item tag
    /// </summary>
    public class Li : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Li(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Li(HtmlNode node) : base(node) { }
    }
}