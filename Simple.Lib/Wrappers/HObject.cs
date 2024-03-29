﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using RafaelEstevam.Simple.Spider.Helper;

namespace RafaelEstevam.Simple.Spider.Wrappers
{
    /// <summary>
    /// Represents a Html Tags Collection
    /// </summary>
    public class HObject : IEnumerable<HObject>
    {
        /// <summary>
        /// Types of items to search
        /// </summary>
        public enum SearchType
        {
            /// <summary>
            /// Search for Tag Name
            /// </summary>
            AnyElement,
            /// <summary>
            /// Search for Tag Name only on child elements
            /// </summary>
            ChildElement,
            /// <summary>
            /// Search for ID attribute
            /// </summary>
            AnyIdEquals,
            /// <summary>
            /// Search for Class attribute
            /// </summary>
            AnyClassContaining,

            /// <summary>
            /// Filter current by ID, similar to this.OfID(Name)
            /// </summary>
            FilterId,
            /// <summary>
            /// Filter current by Class, similar to this.OfClass(Name)
            /// </summary>
            FilterClass,
        }

        private readonly IEnumerable<HtmlNode> nodes;

        /// <summary>
        /// Checks if the filtering results in an empty object
        /// </summary>
        /// <returns>True if there are no elements left</returns>
        public bool IsEmpty()
        {
            return nodes.Count() == 0;
        }
        /// <summary>
        /// Checks if the first element has the specified attribute
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>True if the first element has the Attribute</returns>
        public bool HasAttribute(string AttributeName)
        {
            var n = nodes.FirstOrDefault();
            if (n == null) return false;
            return n.Attributes.Any(a => a.Name == AttributeName);
        }

        /// <summary>
        /// Initializes a new instance of the HObject class
        /// </summary>
        public HObject(HtmlDocument d)
        {
            nodes = new HtmlNode[] { d.DocumentNode };
        }
        /// <summary>
        /// Initializes a new instance of the HObject class
        /// </summary>
        public HObject(HtmlNode n)
        {
            nodes = new HtmlNode[] { n };
        }
        /// <summary>
        /// Initializes a new instance of the HObject class
        /// </summary>
        public HObject(IEnumerable<HtmlNode> ns)
        {
            nodes = ns;
        }

        /// <summary>
        /// Gets a HObject with specified Name matching Type
        /// </summary>
        /// <param name="Name">Name of the element to search for</param>
        /// <param name="Type">Type of search</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[SearchType Type, string Name]
        {
            get
            {
                return Type switch
                {
                    SearchType.AnyElement => Tags(Name),
                    SearchType.ChildElement => Children(Name),
                    SearchType.AnyIdEquals => IDs(Name),
                    SearchType.AnyClassContaining => Classes(Name),
                    SearchType.FilterId => OfID(Name),
                    SearchType.FilterClass => OfClass(Name),
                    _ => throw new ArgumentException("SearchType dows not exists"),
                };
            }
        }
        /// <summary>
        /// Gets a HObject with specified Tag. Supports css notation of '#' for ID and '.' for classes
        /// </summary>
        /// <param name="SearchName">Name of the Tag to search for or the '#{IdName}' or the '.{ClassName}'</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[string SearchName]
        {
            get
            {
                if (SearchName.Contains(">"))
                {
                    return this[SearchName.Split('>')];
                }

                if (SearchName.StartsWith('#'))
                {
                    return this[SearchType.FilterId, SearchName.Substring(1)];
                }
                if (SearchName.StartsWith('.'))
                {
                    return this[SearchType.FilterClass, SearchName.Substring(1)];
                }
                return this[SearchType.AnyElement, SearchName];
            }
        }
        /// <summary>
        /// Gets a HObject searched in chain with an array of Names. Supports css notation of '#' for ID and '.' for classes
        /// </summary>
        /// <param name="SearchNames">Array of names of the Tag to search for or the '#{IdName}' or the '.{ClassName}'</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[params string[] SearchNames]
        {
            get
            {
                var result = this;
                foreach (var n in SearchNames)
                {
                    result = result[n.Trim()];
                }
                return result;
            }
        }
        /// <summary>
        /// Gets the n-th HObject
        /// </summary>
        /// <param name="Position">Index of the HObject to be returned</param>
        /// <returns>HObject at specified position</returns>
        public HObject this[int Position]
        {
            get { return new HObject(nodes.ElementAt(Position)); }
        }

        #region Element Name

        /// <summary>
        /// Returns all Tags named TagName
        /// </summary>
        /// <param name="TagName">The name of the tag</param>
        /// <returns>A HObject filtered</returns>
        public HObject Tags(string TagName)
        {
            var x = nodes
                         .SelectMany(el => el.DescendantsAndSelf()
                                             .Where(d => d.Name == TagName))
                         .ToArray(); // force enumerate
            return new HObject(x);
        }
        /// <summary>
        /// Returns all children tags named TagName
        /// </summary>
        /// <param name="TagName">The name of the tag</param>
        /// <returns>A HObject filtered</returns>
        public HObject Children(string TagName)
        {
            var x = nodes
                         .SelectMany(el => el.ChildNodes
                                             .Where(d => d.Name == TagName))
                         .ToArray(); // force enumerate
            return new HObject(x);
        }

        #endregion

        #region ID Stuff
        /// <summary>
        /// Returns all Tags with ID equals to specified parameter
        /// </summary>
        /// <param name="IDsEquals">Id value to search for</param>
        /// <returns>A HObject</returns>
        public HObject IDs(string IDsEquals)
        {
            return Having("id", IDsEquals);
        }
        /// <summary>
        /// Filters current tags selecting only the ones with specified Id value
        /// </summary>
        /// <param name="IDsEquals">Id value to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfID(string IDsEquals)
        {
            return OfWhich("id", IDsEquals);
        }

        #endregion

        #region Classes
        /// <summary>
        /// Returns all Tags with Class containing specified parameter
        /// Note: In HTML classes are an array of strings
        /// </summary>
        /// <param name="ClassName">The class name to search for</param>
        /// <returns>A HObject</returns>
        public HObject Classes(string ClassName)
        {
            var x = nodes
                      .SelectMany(el => el.DescendantsAndSelf()
                                          .Where(d => d.Attributes
                                                       .Any(a => a.Name == "class" && a.Value.Split(' ').Contains(ClassName))))
                      .ToArray();

            return new HObject(x);
        }
        /// <summary>
        /// Filters current tags selecting only the ones with specified Class value
        /// Note: In HTML classes are an array of strings
        /// </summary>
        /// <param name="ClassName">The class name to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfClass(string ClassName)
        {
            return new HObject(nodes
                .Where(n => n.Attributes.Any(a => a.Name == "class"
                                                  && (a.Value == ClassName || a.Value.Split(' ').Any(av => av == ClassName))))
                .ToArray());
        }

        #endregion

        #region Choose Attribute
        /// <summary>
        /// Returns all Tags with Named Attribute equals to specified parameter. Search in all descendants
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <param name="AttributeValue">Value to search for</param>
        /// <returns>A HObject</returns>
        public HObject Having(string AttributeName, string AttributeValue)
        {
            //return new HObject(xElements.DescendantsAndSelf().Where(x => x.Attribute(AttributeName)?.Value == AttributeValue).ToArray());

            return new HObject(nodes
                .SelectMany(n => n.DescendantsAndSelf())
                .Where(x => x.Attributes.Any(a => a.Name == AttributeName && a.Value == AttributeValue))
                .ToArray());

        }
        /// <summary>
        /// Filters current tags selecting only the ones with Named Attribute equals to specified parameter. Search only in root elements
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <param name="AttributeValue">Value to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfWhich(string AttributeName, string AttributeValue)
        {
            return new HObject(nodes
                .Where(x => x.Attributes.Any(a => a.Name == AttributeName && a.Value == AttributeValue))
                .ToArray());
        }

        /// <summary>
        /// Executes a CSS Select query with '&gt;' separator, '.Class' and '#Id'
        /// </summary>
        /// <param name="Query">CSS Query string</param>
        /// <returns>HObject selected</returns>
        public HObject CssSelect(string Query)
        {
            //return this[Query.Split('>')];
            throw new NotImplementedException();
        }
        /// <summary>
        /// Executes a XPath Select query in each element and returns all results
        /// </summary>
        /// <param name="Query">XPath Query string</param>
        /// <returns>HObject elements with all results</returns>
        public HObject XPathSelect(string Query)
        {
            return new HObject(xpath(Query).ToArray());
        }

        #endregion

        #region Other methods
        private IEnumerable<HtmlNode> xpath(string Query)
        {
            foreach (var n in nodes)
            {
                var sn = n.SelectNodes(Query);
                if (sn == null) continue;
                foreach (var q in sn)
                {
                    yield return q;
                }
            }
        }

        /// <summary>
        /// Returns all Nodes of the collection
        /// </summary>
        public HtmlNode GetNode()
        {
            return nodes.FirstOrDefault();
        }
        /// <summary>
        /// Returns all Nodes of the collection
        /// </summary>
        public IEnumerable<HtmlNode> GetNodes()
        {
            return nodes.ToArray();
        }

        /// <summary>
        /// Returns first element Value
        /// </summary>
        /// <returns>The string Value of the element</returns>
        public string GetValue()
        {
            return nodes.FirstOrDefault()?.InnerText;
        }
        /// <summary>
        /// Returns first element InnerHtml
        /// </summary>
        public string GetHtml()
        {
            return nodes.FirstOrDefault()?.InnerHtml;
        }

        /// <summary>
        /// Returns first element Value trimmed
        /// </summary>
        /// <returns>The trimmed string Value of the element</returns>
        public string Trim()
        {
            return GetValue()?.Trim();
        }
        /// <summary>
        /// Returns an array with all Elements values
        /// </summary>
        /// <returns>String array of the values</returns>
        public string[] GetValues()
        {
            return nodes.Select(x => x.InnerText).ToArray();
        }
        /// <summary>
        /// Returns an array with all InnerHtml values
        /// </summary>
        public string[] GetHtmls()
        {
            return nodes.Select(x => x.InnerHtml).ToArray();
        }
        /// <summary>
        /// Returns an array with all Elements values trimmed
        /// </summary>
        /// <returns>String array of the values trimmed</returns>
        public string[] TrimAll()
        {
            return nodes.Select(x => x.InnerText.Trim()).ToArray();
        }
        /// <summary>
        /// Returns an array with all #text elements contents
        /// </summary>
        public string[] GetTexts()
        {
            //return nodes.SelectMany(x => x.ChildNodes).Select(x => x.InnerText.Trim()).ToArray();
            return getAllNodesAndSelf(nodes)
                .Where(n => n.NodeType == HtmlNodeType.Text)
                .Select(n => n.InnerText)
                .ToArray();
        }
        /// <summary>
        /// Returns all #text nodes concatenated with spaces and lines breaked on `br` with `\n`
        /// </summary>
        public string GetText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var n in getAllNodesAndSelf(nodes))
            {
                if (n.NodeType == HtmlNodeType.Text)
                {
                    if (sb.Length > 0 && !char.IsWhiteSpace(sb[^1]))
                    {
                        sb.Append(' ');
                    }
                    sb.Append(n.InnerText.Trim());
                }
                if (n.Name == "br")
                {
                    sb.Append('\n');
                }

                //.Where(n => n.NodeType == HtmlNodeType.Text)
                //.Select(n => n.InnerText)
                //.ToArray();
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Returns an array with all #text elements contents recursively
        /// </summary>
        public string[] GetInnerTexts()
        {
            return getAllNodesAndSelf(nodes)
                .Where(n => n.NodeType == HtmlNodeType.Text)
                .Select(n => n.InnerText)
                .ToArray();
        }
        /// <summary>
        /// Returns an array with childs #text elements contents
        /// </summary>
        public string[] GetChildTexts()
        {
            return nodes.SelectMany(x => x.ChildNodes)
                        .Where(n => n.NodeType == HtmlNodeType.Text)
                        .Select(x => x.InnerText.Trim())
                        .ToArray();
        }

        /// <summary>
        /// Returns an array with all #text elements contents trimmed
        /// </summary>
        public string[] TrimAllText()
        {
            //return nodes.SelectMany(x => x.ChildNodes).Select(x => x.InnerText.Trim()).ToArray();
            return GetTexts()
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
        }
        /// <summary>
        /// Get all child nodes and self recursively
        /// </summary>
        public IEnumerable<HtmlNode> GetAllChildsNodesAndSelf()
            => getAllNodesAndSelf(nodes);
        /// <summary>
        /// Get all child nodes recursively
        /// </summary>
        public IEnumerable<HtmlNode> GetAllChildsNodes()
            => nodes.SelectMany(n => getAllNodes(n));
        /// <summary>
        /// Get all direct child nodes
        /// </summary>
        public IEnumerable<HtmlNode> GetChildsNodes()
            => nodes.SelectMany(n => n.ChildNodes.Cast<HtmlNode>());

        private static IEnumerable<HtmlNode> getAllNodesAndSelf(IEnumerable<HtmlNode> nodes)
        {
            return nodes.SelectMany(n => getAllNodesAndSelf(n));
        }
        private static IEnumerable<HtmlNode> getAllNodesAndSelf(HtmlNode node)
        {
            yield return node;

            var childs = getAllNodesAndSelf(node.ChildNodes.Cast<HtmlNode>());
            foreach (var c in childs) yield return c;
        }
        private static IEnumerable<HtmlNode> getAllNodes(HtmlNode node)
        {
            return getAllNodesAndSelf(node.ChildNodes.Cast<HtmlNode>());
        }

        /// <summary>
        /// Gets the value of attribute named AttributeName of the first item
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>Value of the attribute</returns>
        public string GetAttributeValue(string AttributeName)
        {
            return nodes.FirstOrDefault()?.GetAttributeValue(AttributeName, (string)null);
        }
        /// <summary>
        /// Gets all the values of the attribute named AttributeName of all items
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>String array of Values</returns>
        public string[] GetAttributeValues(string AttributeName)
        {
            return nodes.Select(n => n.GetAttributeValue(AttributeName, (string)null)).ToArray();
        }

        /// <summary>
        /// Gets the value of attribute 'href' of the first item
        /// </summary>
        /// <returns>String containing the value</returns>
        public string GetHrefValue()
        {
            return GetAttributeValue("href");
        }
        /// <summary>
        /// Gets all the values of the attribute 'href' of all items
        /// </summary>
        /// <returns>String array with values</returns>
        public string[] GetHrefValues()
        {
            return GetAttributeValues("href");
        }
        /// <summary>
        /// Gets the value of the Class attribute of the first item
        /// </summary>
        /// <returns>String containing the Class</returns>
        public string GetClassValue()
        {
            return GetAttributeValue("class");
        }
        /// <summary>
        /// Gets the value of the Id attribute of the first item
        /// </summary>
        /// <returns>String containing the Id</returns>
        public string GetIdValue()
        {
            return GetAttributeValue("id");
        }
        /// <summary>
        /// Gets the value of all the Id attribute of all items
        /// </summary>
        /// <returns>String array containing all the Ids</returns>
        public string[] GetIdsValues()
        {
            return GetAttributeValues("id");
        }
        /// <summary>
        /// Gets the value of the Name attribute of the first item
        /// </summary>
        /// <returns>String containing the Name</returns>
        public string GetNameValue()
        {
            return GetAttributeValue("name");
        }
        /// <summary>
        /// Gets the value of all the Name attribute of all items
        /// </summary>
        /// <returns>String array containing all the Names</returns>
        public string[] GetNameValues()
        {
            return GetAttributeValues("name");
        }

        /// <summary>
        /// Gets the value of the Style attribute of the first item
        /// </summary>
        /// <returns>String containing the Style</returns>
        public string GetStyleValue()
        {
            return GetAttributeValue("style");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the HObject collection.
        /// </summary>
        /// <returns>An HObject enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<HObject> GetEnumerator()
        {
            return nodes.Select(x => new HObject(x)).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns first node of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator HtmlNode(HObject h)
        {
            return h.GetNode();
        }
        /// <summary>
        /// Returns all Nodes of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator HtmlNode[](HObject h)
        {
            return h.GetNodes().ToArray();
        }

        /// <summary>
        /// Returns the Value of the first element of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator string(HObject h)
        {
            return h.GetValue();
        }
        /// <summary>
        /// Returns the Values of all elements of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator string[](HObject h)
        {
            return h.GetValues();
        }
        /// <summary>
        /// Parses from HTML
        /// </summary>
        public static HObject FromHTML(string html)
        {
            var document = HtmlParseHelper.ParseHtmlDocument(html);
            return new HObject(document);
        }
    }
}
