using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html IFrame tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/iframe">HTML element docs</a>
    /// </summary>
    public class IFrame : Tag, ITagSrc, ITagName
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public IFrame(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "iframe"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public IFrame(HtmlNode node) : base(node) { ThrowsIfNotName(node, "iframe"); }

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
