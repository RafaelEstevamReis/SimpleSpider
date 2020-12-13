using System;
using HtmlAgilityPack;
using RafaelEstevam.Simple.Spider.Helper;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Anchor tag
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a">HTML element docs</a>
    /// </summary>
    public class Anchor : Tag
    {
        /// <summary>
        /// Enum for all html anchor target options
        /// </summary>
        public enum Targets
        {
            /// <summary>
            /// (Default) the current browsing context
            /// </summary>
            Self,
            /// <summary>
            /// A new tab or window
            /// </summary>
            Blank,
            /// <summary>
            ///  the parent browsing context of the current one
            /// </summary>
            Parent,
            /// <summary>
            /// the topmost browsing context
            /// </summary>
            Top,
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Anchor(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "a"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Anchor(HtmlNode node) : base(node) { ThrowsIfNotName(node, "a"); }
        /// <summary>
        /// Gets the Href attribute of the tag
        /// </summary>
        public string Href => Attributes["href"];
        /// <summary>
        /// Combines ParentUri with Href and returns a clean Uri
        /// </summary>
        /// <param name="ParentUri">Parent Uri to combine to</param>
        /// <returns>An Uri combined without whitespaces</returns>
        public Uri GetUri(Uri ParentUri)
        {
            return UriHelper.Combine(ParentUri, Href, true);
        }

        /// <summary>
        /// Gets the Download attribute of the tag
        /// </summary>
        public string Download => Attributes["download"];
        /// <summary>
        /// Indicates whenever Download is present or not
        /// </summary>
        public bool IsDownload => Download != null;

        /// <summary>
        /// Gets the Ping attribute of the tag already splitted
        /// </summary>
        public string[] Ping => Attributes["ping"]?.Split(' ');

        /// <summary>
        /// Gets the Target attribute of the tag
        /// </summary>
        public string Target => Attributes["target"];
        /// <summary>
        /// Gets the Target attribute of the tag as Targets enum
        /// </summary>
        public Targets TargetType
        {
            get
            {
                string target = Target;

                if (target.EndsWith("blank", StringComparison.InvariantCultureIgnoreCase))
                    return Targets.Blank;
                if (target.EndsWith("parent", StringComparison.InvariantCultureIgnoreCase))
                    return Targets.Blank;
                if (target.EndsWith("top", StringComparison.InvariantCultureIgnoreCase))
                    return Targets.Blank;

                return Targets.Self;
            }
        }

        /// <summary>
        /// Gets the Type (mime-type) attribute of the tag
        /// </summary>
        public string Type => Attributes["type"];

    }
}
