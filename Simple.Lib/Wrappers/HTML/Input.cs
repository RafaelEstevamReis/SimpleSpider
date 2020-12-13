using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Input tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input">HTML element docs</a>
    /// </summary>
    public class Input : Tag, ITagValue, ITagType, ILabelable
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Input(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "input"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Input(HtmlNode node) : base(node) { ThrowsIfNotName(node, "input"); }

        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Src attribute of the tag
        /// </summary>
        public string Src => Attributes["src"];
        /// <summary>
        /// Gets the Type attribute of the tag
        /// </summary>
        public string Type => Attributes["type"];
        /// <summary>
        /// Gets the Value attribute of the tag
        /// </summary>
        public string Value => Attributes["value"];

        /// <summary>
        /// Gets the Checked attribute of the tag
        /// </summary>
        public bool Checked => Attributes["checked"] != null;
        /// <summary>
        /// Gets the Disabled attribute of the tag
        /// </summary>
        public bool Disabled => Attributes["disabled"] != null;
    }
}
