using System.Linq;
using System.Collections;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html tag attribute collection
    /// </summary>
    public class TagAttributes : IEnumerable<HtmlAttribute>
    {
        private HtmlAttributeCollection attributes;

        /// <summary>
        /// Initializes a new empty instance
        /// </summary>
        public TagAttributes(HtmlAttributeCollection attributes)
        {
            this.attributes = attributes;
        }

        /// <summary>
        /// Checks for existence of attribute with given name
        /// </summary>
        public bool Contains(string Name)
        {
            return attributes.Contains(Name);
        }

        /// <summary>
        /// Gets the value af the attribute named Value
        /// </summary>
        /// <param name="Name">Name of the attribute</param>
        /// <returns>String with the content of the attribute</returns>
        public string this[string Name]
        {
            get
            {
                return attributes[Name]?.Value;
            }
            set
            {
                attributes[Name].Value = value;
            }
        }
        /// <summary>
        /// Enumerates all attributes
        /// </summary>
        public IEnumerator<HtmlAttribute> GetEnumerator()
        {
            foreach (var a in attributes) yield return a;   
        }
        /// <summary>
        /// Enumerator for the attributes
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            return string.Join("; ", attributes.Select(a => $"{a.Name}={a.Value}"));
        }
    }
}
