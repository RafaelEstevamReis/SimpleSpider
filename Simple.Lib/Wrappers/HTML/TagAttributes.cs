using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    /// <summary>
    /// Represents an html tag attribute collection
    /// </summary>
    public class TagAttributes : IEnumerable<TagAttribute>
    {
        /// <summary>
        /// Initializes a new empty instance
        /// </summary>
        public TagAttributes() { }

        /// <summary>
        /// Initializes a new instance from a NameValueCollection
        /// </summary>
        public TagAttributes(NameValueCollection nameValueCollection)
        {
            Items = nameValueCollection
                        .AllKeys.Select(k => new TagAttribute()
                        {
                            Name = k,
                            Value = nameValueCollection[k]
                        })
                        .ToArray();
        }
        /// <summary>
        /// Gets an array with all attributes
        /// </summary>
        public TagAttribute[] Items { get; private set; }
        /// <summary>
        /// Gets the value af the attribute named Value
        /// </summary>
        /// <param name="Name">Name of the attribute</param>
        /// <returns>String with the content of the attribute</returns>
        public string this[string Name]
        {
            get
            {
                return Items.FirstOrDefault(a => a.Name == Name)?.Value;
            }
        }
        /// <summary>
        /// Enumerates all attributes
        /// </summary>
        public IEnumerator<TagAttribute> GetEnumerator()
        {
            return ((IEnumerable<TagAttribute>)Items).GetEnumerator();
        }
        /// <summary>
        /// Enumerator for the attributes
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            return string.Join<TagAttribute>("; ", Items);
        }
    }
    /// <summary>
    /// Represents an html tag attribute
    /// </summary>
    public class TagAttribute
    {
        /// <summary>
        /// Attribute's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Attribute's value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
