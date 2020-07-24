using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

namespace Net.RafaelEstevam.Spider
{
    public class HeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        Dictionary<string, string> dicValues;

        public HeaderCollection(IEnumerable<KeyValuePair<string,string>> kvp)
           : this()
        {
            foreach (var pair in kvp) this[pair.Key] = pair.Value;
        }
        public HeaderCollection(NameValueCollection nvc)
            : this()
        {
            foreach (var k in nvc.AllKeys) this[k] = nvc[k];
        }
        public HeaderCollection()
        {
            dicValues = new Dictionary<string, string>();
        }

        public string this[string Key]
        {
            get
            {
                if (dicValues.ContainsKey(Key)) return dicValues[Key];
                return null;
            }
            set
            {
                dicValues[Key] = value;
            }
        }

        public string[] AllKeys
        {
            get { return dicValues.Keys.ToArray(); }
        }

        [XmlIgnore]
        public KeyValuePair<string, string>[] Values
        {
            get
            {
                return dicValues.ToArray();
            }
            set
            {
                // ignores duplicate keys
                foreach (var pair in value)
                {
                    dicValues[pair.Key] = pair.Value;
                }
            }
        }

        [XmlArray("Pairs")]
        [XmlArrayItem("Pair")]
        public SerializableKeyValuePair[] Pairs
        {
            get
            {
                return dicValues.Select(o => (SerializableKeyValuePair)o).ToArray();
            }
            set
            {
                // ignores duplicate keys
                foreach (var pair in value)
                {
                    dicValues[pair.Key] = pair.Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dicValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dicValues.GetEnumerator();
        }

    }
    public class SerializableKeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public static implicit operator KeyValuePair<string, string>(SerializableKeyValuePair pair)
        {
            return new KeyValuePair<string, string>(pair.Key, pair.Value);
        }
        public static implicit operator SerializableKeyValuePair(KeyValuePair<string, string> pair)
        {
            return new SerializableKeyValuePair() { Key = pair.Key, Value = pair.Value };
        }
    }
}