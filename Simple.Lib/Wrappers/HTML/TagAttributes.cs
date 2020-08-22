using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class TagAttributes : IEnumerable<TagAttribute>
    {
        public TagAttributes() { }

        public TagAttributes(NameValueCollection nameValueCollection)
        {
            Attributes = nameValueCollection
                        .AllKeys.Select(k => new TagAttribute()
                        {
                            Name = k,
                            Value = nameValueCollection[k]
                        })
                        .ToArray();
        }

        public TagAttribute[] Attributes { get; private set; }

        public string this[string Name]
        {
            get
            {
                return Attributes.FirstOrDefault(a => a.Name == Name)?.Value;
            }
        }

        public IEnumerator<TagAttribute> GetEnumerator()
        {
            return ((IEnumerable<TagAttribute>)Attributes).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join<TagAttribute>("; ", Attributes);
        }
    }
    public class TagAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
