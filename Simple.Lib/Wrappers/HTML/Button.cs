using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Button tag
    /// </summary>
    public class Button : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Button(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Button(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Type attribute of the tag
        /// </summary>
        public string Type => Attributes["type"];
        /// <summary>
        /// Gets the Value attribute of the tag
        /// </summary>
        public string Value => Attributes["value"];
        /// <summary>
        /// Gets the Disabled attribute of the tag
        /// </summary>
        public bool Disabled => Attributes["disabled"] != null;
    }
}
