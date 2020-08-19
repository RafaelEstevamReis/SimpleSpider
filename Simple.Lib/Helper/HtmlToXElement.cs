using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Class to convert Html to XElement, uses HtmlAgilityPack
    /// </summary>
    public class HtmlToXElement
    {
        private static readonly char[] InvalidCharsToRemove = { ':' };

        /// <summary>
        /// XElement parse modes
        /// </summary>
        public enum XElementParser
        {
            /// <summary>
            /// Saves a Stream with the document and parse with XElement.Parse
            /// </summary>
            LoadFromXmlReader,
            /// <summary>
            /// Recursively iterate over elements and converts to XElement
            /// </summary>
            RecursiveNodeParser,
        }

        /// <summary>
        /// Search for invalid Attribute names and remove them
        /// </summary>
        public static bool DefaultSearchForInvalidNames { get; set; } = false;
        /// <summary>
        /// Search for Script tags and remove them
        /// </summary>
        public static bool DefaultSearchAndRemoveScripts { get; set; } = false;
        /// <summary>
        /// Search for Style tags and remove them
        /// </summary>
        public static bool DefaultSearchAndRemoveStyleElements { get; set; } = false;
        /// <summary>
        /// Search for Comment blocks and remove them
        /// </summary>
        public static bool DefaultSearchAndRemoveComments { get; set; } = false;
        /// <summary>
        /// Get os sets when text blocks should be trimmed
        /// </summary>
        public static bool DefaultTrimTextBlocks { get; set; } = false;
        /// <summary>
        /// Sets the default XElement parsing mode
        /// </summary>
        public static XElementParser DefaultXElementParserMode { get; set; } = XElementParser.LoadFromXmlReader;

        /// <summary>
        /// Parses an HTML as a XElement with default options
        /// </summary>
        /// <param name="html">Html content to be parsed</param>
        /// <returns>XElement parsed</returns>
        public static XElement Parse(string html)
        {
            return Parse(html, ParseOptions.Defaults());
        }
        /// <summary>
        /// Parses an HTML as a XElement
        /// </summary>
        /// <param name="html">Html content to be parsed</param>
        /// <param name="options">Parse options</param>
        /// <returns>XElement parsed</returns>
        public static XElement Parse(string html, ParseOptions options)
        {
            return Parse(HtmlParseHelper.ParseHtmlDocument(html), options);
        }
        /// <summary>
        /// Parses an HtmlDocument as a XElement
        /// </summary>
        /// <param name="doc">HtmlDocument to be processed</param>
        /// <param name="options">Parse options</param>
        /// <returns>XElement returned</returns>
        public static XElement Parse(HtmlDocument doc, ParseOptions options)
        {
            if (options.SearchAndRemoveScripts)
            {
                foreach (var e in doc.DocumentNode.Descendants("script").ToArray())
                {
                    e.Remove();
                }
            }
            if (options.SearchAndRemoveComments)
            {
                foreach (var e in doc.DocumentNode.Descendants("#comment").ToArray())
                {
                    e.Remove();
                }
            }
            if (options.SearchAndRemoveStyleElements)
            {
                foreach (var e in doc.DocumentNode.Descendants("style").ToArray())
                {
                    e.Remove();
                }
            }

            if (options.XElementParserMode == XElementParser.RecursiveNodeParser)
            {
                return processAsRecursive(doc, options);
            }
            else if (options.XElementParserMode == XElementParser.LoadFromXmlReader)
            {
                return processAsXmlReader(doc, options);
            }
            else
            {
                throw new InvalidOperationException("Invalid XElementParserMode");
            }
        }

        private static XElement processAsXmlReader(HtmlDocument doc, ParseOptions options)
        {
            if (options.SearchForInvalidNames)
            {
                foreach (var n in doc.DocumentNode.Descendants())
                {
                    foreach (var att in n.Attributes.ToArray()) // Creates a copy to remove
                    {
                        if (InvalidCharsToRemove.Any(c => att.Name.Contains(c)))
                        {
                            n.Attributes.Remove(att);
                            continue;
                        }
                    }
                }
            }

            using StringWriter writer = new StringWriter();
            doc.Save(writer);
            string dom = writer.ToString();
            using StringReader reader = new StringReader(dom);
            var x = XElement.Load(reader);

            //remove Span root
            if (x.Name.LocalName == "span")
            {
                XElement fn;
                if (x.FirstNode is XComment) fn = (XElement)x.FirstNode.NextNode;
                else fn = x.FirstNode as XElement;

                if (fn != null && fn.Name.LocalName == "html")
                {
                    return fn;
                }
            }
            return x;
        }
        private static XElement processAsRecursive(HtmlDocument doc, ParseOptions options)
        {

            XElement root = createXElement(doc.DocumentNode, options);
            if (root.Name == "document" || root.Name == "span")
            {
                XElement fn = (XElement)root.FirstNode;
                if (fn.Name.LocalName == "html")
                {
                    return fn;
                }
            }
            return root;
        }

        private static XElement createXElement(HtmlNode node, ParseOptions options)
        {
            XElement x = null;
            var elements = getNodes(node, options).ToArray();
            if (elements.Length == 1 && elements[0].Name == "text")
            {
                x = new XElement(getName(node.Name));
                x.Value = elements[0].Value;
            }
            if (node.Name == "#document")
            {
                if (elements.Length == 1) x = elements[0];
                else
                {
                    x = new XElement(getName(node.Name), elements);
                }
            }
            else
            {
                x = new XElement(getName(node.Name), elements);
            }

            foreach (var a in node.Attributes)
            {
                string name = a.Name;
                if (name.Contains(':')) name = name.Replace(':', '_');
                x.SetAttributeValue(name, a.Value);
            }

            return x;
        }
        private static IEnumerable<XElement> getNodes(HtmlNode node, ParseOptions options)
        {
            foreach (var c in node.ChildNodes)
            {
                var e = createXElement(c, options);
                if (c.Name == "#text")
                {
                    if (c.InnerText.Trim().Length == 0) continue;

                    e.Value = c.InnerText;

                    if (options.TrimTextBlocks) e.Value = e.Value.Trim();
                }
                if (c.Name == "#comment" && c.InnerText.Trim().Length == 0) continue;
                yield return e;
            }
        }
        private static XName getName(string name)
        {
            if (string.IsNullOrEmpty(name)) return XName.Get("_Blank");
            if (name[0] == '#') return XName.Get(name.Substring(1));
            return XName.Get(name);
        }
        /// <summary>
        /// Defines parsing options
        /// </summary>
        public class ParseOptions
        {
            /// <summary>
            /// Search for invalid Attribute names and remove them
            /// </summary>
            public bool SearchForInvalidNames { get; set; } = false;
            /// <summary>
            /// Search for Script tags and remove them
            /// </summary>
            public bool SearchAndRemoveScripts { get; set; } = false;
            /// <summary>
            /// Search for Style tags and remove them
            /// </summary>
            public bool SearchAndRemoveStyleElements { get; set; } = false;
            /// <summary>
            /// Search for Comment blocks and remove them
            /// </summary>
            public bool SearchAndRemoveComments { get; set; } = false;
            /// <summary>
            /// Sets the XElement parsing mode
            /// </summary>
            public XElementParser XElementParserMode { get; set; } = XElementParser.LoadFromXmlReader;
            /// <summary>
            /// Get os sets when text blocks should be trimmed
            /// </summary>
            public bool TrimTextBlocks { get; set; } = false;

            internal static ParseOptions Defaults()
            {
                return new ParseOptions()
                {
                    SearchForInvalidNames = DefaultSearchForInvalidNames,
                    SearchAndRemoveComments = DefaultSearchAndRemoveComments,
                    SearchAndRemoveScripts = DefaultSearchAndRemoveScripts,
                    SearchAndRemoveStyleElements = DefaultSearchAndRemoveStyleElements,
                    TrimTextBlocks = DefaultTrimTextBlocks,
                    XElementParserMode = DefaultXElementParserMode
                };
            }
        }
    }
}
