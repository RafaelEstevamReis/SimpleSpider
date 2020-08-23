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
            Items = nameValueCollection
                        .AllKeys.Select(k => new TagAttribute()
                        {
                            Name = k,
                            Value = nameValueCollection[k]
                        })
                        .ToArray();
        }

        public TagAttribute[] Items { get; private set; }

        public string this[string Name]
        {
            get
            {
                return Items.FirstOrDefault(a => a.Name == Name)?.Value;
            }
        }

        public IEnumerator<TagAttribute> GetEnumerator()
        {
            return ((IEnumerable<TagAttribute>)Items).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join<TagAttribute>("; ", Items);
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
