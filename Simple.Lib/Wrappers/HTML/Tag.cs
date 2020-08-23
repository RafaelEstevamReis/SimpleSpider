using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Tag
    {
        /// <summary>
        /// Exposes underlying HtmlNode
        /// </summary>
        public HtmlNode Node { get; }

        public Tag(HtmlDocument document)
        {
            if (document == null) throw new ArgumentNullException("Document must be not null");

            Node = document.DocumentNode.SelectSingleNode("html");
            if (Node == null) Node = document.DocumentNode;
        }
        public Tag(HtmlNode node)
        {
            if (node == null) throw new ArgumentNullException("Node must be not null");

            Node = node;
        }

        public string TagName => Node.Name;
        public string InnerText => Node.InnerText;

        public string Id => Node.Id;
        public string Class => Attributes["class"];
        public string Style => Attributes["style"];
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
        public TagAttributes Attributes
        {
            get
            {
                if (attributes == null) attributes = new TagAttributes(GetAttributes());
                return attributes;
            }
        }

        Tag[] childs = null;
        public Tag[] Childs
        {
            get
            {
                if (childs == null) childs = GetChilds().ToArray();
                return childs;
            }
        }

        public Tag SelectTag(string XPath)
        {
            var n = Node.SelectSingleNode(XPath);
            if (n == null) return null;
            return new Tag(n);
        }
        public IEnumerable<Tag> SelectTags(string XPath)
        {
            var n = Node.SelectNodes(XPath);
            if (n == null) return null;
            return n.Select(o => new Tag(o));
        }

        public IEnumerable<Tag> GetChilds()
        {
            return Node
                .ChildNodes
                .Where(n => n.Name != "#text" /*|| !string.IsNullOrWhiteSpace(n.InnerHtml)*/)
                .Where(n => n.Name != "#comment" || !string.IsNullOrWhiteSpace(n.InnerHtml))
                .Select(n => new Tag(n));
        }
        public IEnumerable<Tag> GetChilds(string TagName)
        {
            return GetChilds().Where(t => t.TagName == TagName);
        }
        public IEnumerable<T> GetChilds<T>() where T : Tag
        {
            if (typeof(T) == typeof(Anchor)) return (IEnumerable<T>)GetChilds("a").Select(t => new Anchor(t.Node));
            if (typeof(T) == typeof(Button)) return (IEnumerable<T>)GetChilds("button").Select(t => new Button(t.Node));
            if (typeof(T) == typeof(Div)) return (IEnumerable<T>)GetChilds("div").Select(t => new Div(t.Node));
            if (typeof(T) == typeof(Form)) return (IEnumerable<T>)GetChilds("form").Select(t => new Form(t.Node));
            if (typeof(T) == typeof(IFrame)) return (IEnumerable<T>)GetChilds("iframe").Select(t => new IFrame(t.Node));
            if (typeof(T) == typeof(Img)) return (IEnumerable<T>)GetChilds("img").Select(t => new Img(t.Node));
            if (typeof(T) == typeof(Input)) return (IEnumerable<T>)GetChilds("input").Select(t => new Input(t.Node));
            if (typeof(T) == typeof(Ol)) return (IEnumerable<T>)GetChilds("ol").Select(t => new Ol(t.Node));
            if (typeof(T) == typeof(Ul)) return (IEnumerable<T>)GetChilds("ul").Select(t => new Ul(t.Node));
            if (typeof(T) == typeof(Li)) return (IEnumerable<T>)GetChilds("li").Select(t => new Li(t.Node));
            if (typeof(T) == typeof(Link)) return (IEnumerable<T>)GetChilds("link").Select(t => new Link(t.Node));
            if (typeof(T) == typeof(Meta)) return (IEnumerable<T>)GetChilds("meta").Select(t => new Meta(t.Node));
            if (typeof(T) == typeof(Select)) return (IEnumerable<T>)GetChilds("select").Select(t => new Select(t.Node));
            if (typeof(T) == typeof(Option)) return (IEnumerable<T>)GetChilds("option").Select(t => new Option(t.Node));

            if (typeof(T) == typeof(Tag)) return (IEnumerable<T>)GetChilds();

            throw new InvalidCastException();
        }
        public NameValueCollection GetAttributes()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (var a in Node.Attributes) nvc[a.Name] = a.Value;
            return nvc;
        }

        public override string ToString()
        {
            return $"<{TagName} {string.Join(" ", Node.Attributes.Select(a => a.Name + "=" + a.Value))}>";
        }

        public static implicit operator HObject(Tag tag)
        {
            return new HObject(tag.Node);
        }
        public static implicit operator Tag(HObject hObj)
        {
            return new Tag(hObj.GetNode());
        }
    }
}
