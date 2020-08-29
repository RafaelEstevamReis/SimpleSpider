﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents a HTML tag
    /// </summary>
    public class Tag
    {
        public static (string, Type)[] MappingTable ={
            ("a",typeof(Anchor)),
            ("button",typeof(Button)),
            ("div",typeof(Div)),
            ("form",typeof(Form)),
            ("iframe",typeof(IFrame)),
            ("img",typeof(Img)),
            ("input",typeof(Input)),
            ("ol",typeof(Ol)),
            ("ul",typeof(Ul)),
            ("li",typeof(Li)),
            ("link",typeof(Link)),
            ("meta",typeof(Meta)),
            ("select",typeof(Select)),
            ("option",typeof(Option)),
        };

        /// <summary>
        /// Exposes underlying HtmlNode
        /// </summary>
        public HtmlNode Node { get; }
        /// <summary>
        /// Initializes a new instance of the Tag class
        /// </summary>
        /// <param name="document">A HtmlDocument to initialize from</param>
        public Tag(HtmlDocument document)
        {
            if (document == null) throw new ArgumentNullException("Document must be not null");

            Node = document.DocumentNode.SelectSingleNode("html");
            if (Node == null) Node = document.DocumentNode;
        }
        /// <summary>
        /// Initializes a new instance of the Tag class
        /// </summary>
        /// <param name="node">A HtmlNode to initialize from</param>
        public Tag(HtmlNode node)
        {
            if (node == null) throw new ArgumentNullException("Node must be not null");

            Node = node;
        }
        /// <summary>
        /// Contains the name of the element
        /// </summary>
        public string TagName => Node.Name;
        /// <summary>
        /// Contains the Inner Text of the element
        /// </summary>
        public string InnerText => Node.InnerText;
        /// <summary>
        /// Contains the Id attribute of the element
        /// </summary>
        public string Id => Node.Id;
        /// <summary>
        /// Contains the Class attribute of the element
        /// </summary>
        public string Class => Attributes["class"];
        /// <summary>
        /// Contains the Style attribute of the element
        /// </summary>
        public string Style => Attributes["style"];
        /// <summary>
        /// Gets an array of the Classes splitted by a space
        /// </summary>
        public string[] Classes
        {
            get
            {
                string cl = Class;
                if (cl == null) return new string[0];
                return cl.Split(' ');
            }
        }

        TagAttributes attributes = null;
        /// <summary>
        /// Gets a TagAttribute with the attributes of the element
        /// </summary>
        public TagAttributes Attributes
        {
            get
            {
                if (attributes == null) attributes = new TagAttributes(GetAttributes());
                return attributes;
            }
        }

        Tag[] children = null;
        /// <summary>
        /// Gets element's children
        /// </summary>
        public Tag[] Children
        {
            get
            {
                if (children == null) children = GetChildren().ToArray();
                return children;
            }
        }
        /// <summary>
        /// Selects an single element using XPath
        /// </summary>
        /// <param name="XPath">A string with the XPath query</param>
        /// <returns>A tag with selected element or null if none matched</returns>
        public Tag SelectTag(string XPath)
        {
            var n = Node.SelectSingleNode(XPath);
            if (n == null) return null;
            return new Tag(n);
        }
        /// <summary>
        /// Selects elements using XPath
        /// </summary>
        /// <param name="XPath">A string with the XPath query</param>
        /// <returns>An Tag collection with selected elements</returns>
        public IEnumerable<Tag> SelectTags(string XPath)
        {
            var n = Node.SelectNodes(XPath);
            if (n == null) return new Tag[0]; // Empty
            return n.Select(o => new Tag(o));
        }
        /// <summary>
        /// Enumerate all Children
        /// </summary>
        public IEnumerable<Tag> GetChildren()
        {
            return Node
                .ChildNodes
                .Where(n => n.Name != "#text" /*|| !string.IsNullOrWhiteSpace(n.InnerHtml)*/)
                .Where(n => n.Name != "#comment" || !string.IsNullOrWhiteSpace(n.InnerHtml))
                .Select(n => new Tag(n));
        }
        /// <summary>
        /// Enumerate all Children with specified TagName
        /// </summary>
        /// <param name="TagName">The name of the tags</param>
        public IEnumerable<Tag> GetChildren(string TagName)
        {
            return GetChildren().Where(t => t.TagName == TagName);
        }
        /// <summary>
        /// Returns all Children of specified {T} Tag
        /// </summary>
        /// <typeparam name="T">Parameter type to be returned</typeparam>
        public IEnumerable<T> GetChildren<T>() where T : Tag
        {
            foreach (var p in MappingTable)
            {
                if (typeof(T) == p.Item2) return invokeChildrenMappingTuple<T>(p);
            }
            throw new InvalidCastException("Requested type is not mapped");
        }
        private IEnumerable<T> invokeChildrenMappingTuple<T>((string, Type) p) where T : Tag
        {
            return GetChildren(p.Item1).Select(t => (T)invokeTag(p.Item2, t.Node));
        }
        private static Tag invokeTag(Type type, HtmlNode node)
        {
            var ctor = type.GetConstructor(new Type[] { typeof(HtmlNode) });
            var tag = ctor.Invoke(new object[] { node });
            return (Tag)tag;
        }
        public T Cast<T>() where T : Tag
        {
            foreach (var p in MappingTable)
            {
                if (typeof(T) == p.Item2)
                {
                    if (p.Item1 == TagName)
                    {
                        return (T)invokeTag(p.Item2, Node);
                    }
                    throw new InvalidCastException("Incorrect type");
                }
            }
            throw new InvalidCastException("Requested type is not mapped");
        }

        /// <summary>
        /// Returns an all attributes as a NameValueCollection 
        /// </summary>
        public NameValueCollection GetAttributes()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (var a in Node.Attributes) nvc[a.Name] = a.Value;
            return nvc;
        }
        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            return $"<{TagName} {string.Join(" ", Node.Attributes.Select(a => a.Name + "=" + a.Value))}>";
        }
        /// <summary>
        /// Implicitly converts between Tag and HObject
        /// </summary>
        public static implicit operator HObject(Tag tag)
        {
            return new HObject(tag.Node);
        }
        /// <summary>
        /// Implicitly converts between Tag and HObject
        /// </summary>
        public static implicit operator Tag(HObject hObj)
        {
            return new Tag(hObj.GetNode());
        }
    }
}
