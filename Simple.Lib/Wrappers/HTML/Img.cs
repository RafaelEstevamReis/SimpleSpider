﻿using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Image tag
    /// </summary>
    public class Img : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Img(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Img(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Src attribute of the tag
        /// </summary>
        public string Src => Attributes["src"];
        /// <summary>
        /// Gets the Alt attribute of the tag
        /// </summary>
        public string Alt => Attributes["alt"];
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
