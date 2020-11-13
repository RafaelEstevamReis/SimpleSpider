using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Helper to do stuff with html Form tags 
    /// </summary>
    [Obsolete("Use Wrappers.HTML.Form instead")]
    public class FormsHelper
    {
        /// <summary>
        /// Enumerate all forms
        /// </summary>
        public static IEnumerable<XElement> GetForms(XElement Root)
        {
            return Root.XPathSelectElements(".//form");
        }
        /// <summary>
        /// Construct a FormTag from a Form XElement
        /// </summary>
        public static FormTag GetFormTag(XElement Form)
        {
            if (Form.Name.LocalName != "form") throw new ArgumentException("Element is not a form");
            return new FormTag(Form);
        }

    }
    /// <summary>
    /// Class to represent a HtmlForm to easy Linq operations
    /// </summary>
    [Obsolete("Use Wrappers.HTML.Form instead")]
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
        /// <summary>
        /// Represents the method attribute options
        /// </summary>
        public enum Methods
        {
            /// <summary>
            /// Http GET method
            /// </summary>
            Get,
            /// <summary>
            /// Http POST method
            /// </summary>
            Post
        }
        /// <summary>
        /// Original XElement parsed
        /// </summary>
        public XElement Element { get; }
        /// <summary>
        /// Represents the Id attribute of the form
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Represents the Name attribute of the form
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Represents the Action attribute of the form
        /// </summary>
        public string Action { get; }
        /// <summary>
        /// Represents the Class attribute of the form
        /// </summary>
        public string Class { get; }
        /// <summary>
        /// Classes splitted by a Space
        /// </summary>
        public string[] Classes { get; }
        /// <summary>
        /// Represents the method attribute of the form
        /// </summary>
        public Methods Method { get; }
        /// <summary>
        /// All inputs elements parsed as InputTag
        /// </summary>
        public InputTag[] Inputs { get; }
        /// <summary>
        /// All buttons XElement
        /// </summary>
        public XElement[] Buttons { get; }
        /// <summary>
        /// Collection of the Hidden fields
        /// </summary>
        public NameValueCollection Hiddens { get; }
        /// <summary>
        /// String representation of the object
        /// </summary>
        public override string ToString()
        {
            return $"Form Name={Name} Method={Method} Action={Action}";
        }
    }
    /// <summary>
    /// Class to represent a InputTag
    /// </summary>
    [Obsolete("Use Wrappers.HTML.Input instead")]
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
        /// <summary>
        /// Represents the Id attribute
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Represents the Type attribute
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// Represents the Name attribute
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Represents the Value attribute
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// Original XElement used to parse
        /// </summary>
        public XElement Element { get; }

        /// <summary>
        /// String representation of the object
        /// </summary>
        public override string ToString()
        {
            return $"Input Name={Name} Type={Type} Name={Name} Value={Value}";
        }
    }
}
