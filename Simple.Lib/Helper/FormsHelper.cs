using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Net.RafaelEstevam.Spider.Helper
{
    public class FormsHelper
    {
        public static IEnumerable<XElement> GetForms(XElement Root)
        {
            return Root.XPathSelectElements(".//form");
        }
        public static FormTag GetFormTag(XElement Form)
        {
            if (Form.Name.LocalName != "form") throw new ArgumentException("Element is not a form");
            return new FormTag(Form);
        }

    }
    public class FormTag
    {
        internal FormTag(XElement element)
        {
            this.Element = element;
            this.Action = element.Attribute("action")?.Value;
            this.Name = element.Attribute("name")?.Value;
            this.Id = element.Attribute("id")?.Value;
            this.Class = element.Attribute("class")?.Value;
            this.Classes = Class?.Split(' ');
            this.Method = Methods.Get;
            if (element.Attribute("method")?.Value?.ToUpper() == "POST") this.Method = Methods.Post;

            this.Inputs = element.XPathSelectElements(".//input").Select(i => new InputTag(i)).ToArray();
            this.Buttons = element.XPathSelectElements(".//button").ToArray();
            
            this.Hiddens = new NameValueCollection();
            foreach (var input in Inputs) 
            {
                if (input.Type != "hidden") continue;
                Hiddens.Add(input.Name, input.Value);
            }
        }

        public enum Methods
        {
            Get,
            Post
        }
        public XElement Element { get; }
        public string Id { get; }
        public string Name { get; }
        public string Action { get; }
        public string Class { get; }
        public string[] Classes { get; }
        public Methods Method { get; }

        public InputTag[] Inputs { get; }
        public XElement[] Buttons { get; }
        public NameValueCollection Hiddens { get; }

        public override string ToString()
        {
            return $"Form Name={Name} Method={Method} Action={Action}";
        }
    }
    public class InputTag
    {
        internal InputTag(XElement element)
        {
            this.Id = element.Attribute("id")?.Value;
            this.Type = element.Attribute("type")?.Value;
            this.Name = element.Attribute("name")?.Value;
            this.Value = element.Attribute("value")?.Value;
            this.Element = element;
        }
        public string Id { get; }
        public string Type { get; }
        public string Name { get; }
        public string Value { get; }
        public XElement Element { get; }

        public override string ToString()
        {
            return $"Input Name={Name} Type={Type} Name={Name} Value={Value}";
        }
    }
}
