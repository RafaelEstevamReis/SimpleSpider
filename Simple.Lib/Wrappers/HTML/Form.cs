using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Tag tag
    /// </summary>
    public class Form : Tag
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Form(HtmlDocument doc) : base(doc) { }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Form(HtmlNode node) : base(node) { }

        /// <summary>
        /// Gets the Action attribute of the tag
        /// </summary>
        public string Action => Attributes["action"];
        /// <summary>
        /// Gets the Method attribute of the tag
        /// </summary>
        public string Method => Attributes["method"];
        /// <summary>
        /// Gets the Name attribute of the tag
        /// </summary>
        public string Name => Attributes["name"];
        /// <summary>
        /// Gets the Target attribute of the tag
        /// </summary>
        public string Target => Attributes["target"];

        /// <summary>
        /// Gets all children input tags
        /// </summary>
        public Input[] GetInputs()
        {
            return GetChildren<Input>().ToArray();
        }
    }
}
