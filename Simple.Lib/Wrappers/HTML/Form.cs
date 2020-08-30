using HtmlAgilityPack;
using System.Collections.Specialized;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html Tag tag
    /// </summary>
    public class Form : Tag, ITagName
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Form(HtmlDocument doc) : base(doc) { ThrowsIfNotName(doc, "form"); }
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public Form(HtmlNode node) : base(node) { ThrowsIfNotName(node, "form"); }

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
            return SelectTags("//input")
                   .Select(t => t.Cast<Input>())
                   .ToArray();
        }
        /// <summary>
        /// Gets all children select tags
        /// </summary>
        public Select[] GetSelects()
        {
            return SelectTags("//select")
                   .Select(t => t.Cast<Select>())
                   .ToArray();
        }

        /// <summary>
        /// Get all form inputs' Name and Value attributes. 
        /// NameValueCollection does not support multiple values with the same key
        /// </summary>
        public NameValueCollection GetFormData()
        {
            var nvc = new NameValueCollection();

            foreach (var i in GetInputs())
            {
                if (string.IsNullOrEmpty(i.Name)) continue;
                if (i.Value == null) continue;

                if (i.Type == "checkbox" && !i.Checked) continue;

                nvc[i.Name] = i.Value;
            }
            foreach (var s in GetSelects())
            {
                if (string.IsNullOrEmpty(s.Name)) continue;

                string value = s.SelectedValue();
                if (value == null) continue;
                nvc[s.Name] = value;
            }

            return nvc;
        }
    }
}
