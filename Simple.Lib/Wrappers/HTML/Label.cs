using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Label tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/label">HTML element docs</a>
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
                if (forElement == null && string.IsNullOrEmpty(For))
                {
                    // May be an direct Input element
                    var inp = SelectTag<Input>("./input");

                    if (inp != null) forElement = inp;

                    return inp;
                }
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