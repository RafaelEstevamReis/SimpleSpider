﻿using System.Linq;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Select tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/select">HTML element docs</a>
    /// </summary>
    public class Select : Tag, ITagName, ILabelable
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Select(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "select"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Select(HtmlNode node) : base(node) { ThrowsIfNotName(node, "select"); }

        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Multiple attribute of the tag
        /// </summary>
        public bool Multiple => Attributes["multiple"] != null;
        /// <summary>
        /// Returns the 'Value' of the first Options with Selected attribute present
        /// </summary>
        /// <returns>The value of the option</returns>
        public string SelectedValue()
        {
            return GetItems().FirstOrDefault(i => i.Selected)?.Value;
        }

        /// <summary>
        /// Gets all children Option tags
        /// </summary>
        public Option[] GetItems()
        {
            return GetChildren<Option>().ToArray();
        }
    }
}
