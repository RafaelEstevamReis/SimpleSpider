﻿using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Anchor tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/meta">HTML element docs</a>
    /// </summary>
    public class Meta : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Meta(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "meta"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Meta(HtmlNode node) : base(node) { ThrowsIfNotName(node, "meta"); }

        /// <summary>
        /// Gets the Charset attribute of the tag
        /// </summary>
        public string Charset => Attributes["charset"];
        /// <summary>
        /// Gets the Http-Equiv attribute of the tag
        /// </summary>
        public string HttpEquiv => Attributes["http-equiv"];
        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Content attribute of the tag
        /// </summary>
        public string Content => Attributes["content"];
        /// <summary>
        /// Gets the Content attribute splitted on comma
        /// </summary>
        public string[] ContentItems
        {
            get
            {
                string cnt = Content;
                if (cnt == null) return new string[0];
                return Content.Split(',');
            }
        }
    }
}
