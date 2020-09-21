using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Label tag
    /// </summary>
    public class Label : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Label(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "label"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Label(HtmlNode node) : base(node) { ThrowsIfNotName(node, "label"); }

        /// <summary>
        /// Gets the for attribute of the tag as Text
        /// </summary>
        public string For => Attributes["for"];
        
        ILabelable forElement;
        /// <summary>
        /// Gets the element indicated in the For attribute of the tag
        /// </summary>
        public ILabelable ForElement
        {
            get
            {
                // Don't have one
                if (forElement == null && string.IsNullOrEmpty(For)) return null;
                // May have one ... (Lazy loaded)
                if (forElement == null)
                {
                    forElement = SelectTag(@$".//*[@id=""{For}""]") as ILabelable;
                    // try load
                }
                return forElement;
            }
        }
    }
}