﻿using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html IFrame tag
    /// </summary>
    public class IFrame : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public IFrame(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public IFrame(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Src attribute of the tag
        /// </summary>
        public string Src => Attributes["src"];

        /// <summary>
        /// Gets the Height attribute of the tag
        /// </summary>
        public int Height
        {
            get
            {
                var h = Attributes["height"];
                if (h == null) return 150; // w3 default
                return int.Parse(h);
            }
        }
        /// <summary>
        /// Gets the Width attribute of the tag
        /// </summary>
        public int Width
        {
            get
            {
                var h = Attributes["width"];
                if (h == null) return 300; // w3 default
                return int.Parse(h);
            }
        }
    }
}