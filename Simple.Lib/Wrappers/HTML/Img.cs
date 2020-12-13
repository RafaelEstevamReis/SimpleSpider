using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Image tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/img">HTML element docs</a>
    /// </summary>
    public class Img : Tag, ITagSrc
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Img(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "img"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Img(HtmlNode node) : base(node) { ThrowsIfNotName(node, "img"); }

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
