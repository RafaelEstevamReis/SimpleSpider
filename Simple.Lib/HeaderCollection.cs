using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Class to hold HTTP Header data
    /// </summary>
    public class HeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        // fast read
        Dictionary<string, string> dicValues;
        /// <summary>
        /// Creates a new object with specified parameter
        /// </summary>
        /// <param name="kvp">Enumerable of KeyValuePairs</param>
        public HeaderCollection(IEnumerable<KeyValuePair<string,string>> kvp)
           : this()
        {
            foreach (var pair in kvp) this[pair.Key] = pair.Value;
        }
        /// <summary>
        /// Creates a new object with specified parameter
        /// </summary>
        /// <param name="nvc">A NameValueCollection to initialize from</param>
        public HeaderCollection(NameValueCollection nvc)
            : this()
        {
            foreach (var k in nvc.AllKeys) this[k] = nvc[k];
        }
        /// <summary>
        /// Creates a new empty object
        /// </summary>
        public HeaderCollection()
        {
            dicValues = new Dictionary<string, string>();
        }
        /// <summary>
        /// Access values with given Key
        /// </summary>
        /// <param name="Key">Key to search for</param>
        /// <returns>Value if found or NULL if key was not present</returns>
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

        /// <summary>
        /// Gets all keys
        /// </summary>
        public string[] AllKeys
        {
            get { return dicValues.Keys.ToArray(); }
        }

        /// <summary>
        /// Gets or sets all pairs as SerializableKeyValuePair, used by XmlSerialization
        /// </summary>
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
        /// <summary>
        /// Enumerates through all pairs
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dicValues.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return dicValues.GetEnumerator();
        }
        /// <summary>
        /// Fancy exhibition on debug
        /// </summary>
        public override string ToString()
        {
            return $"HeaderCollection[{dicValues.Count}]";
        }
    }
    /// <summary>
    /// Represents a Serializable KeyValuePair
    /// </summary>
    public class SerializableKeyValuePair
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Implicit conversion to KeyValuePair&lt;string, string&gt;
        /// </summary>
        public static implicit operator KeyValuePair<string, string>(SerializableKeyValuePair pair)
        {
            return new KeyValuePair<string, string>(pair.Key, pair.Value);
        }
        /// <summary>
        /// Implicit conversion from KeyValuePair&lt;string, string&gt;
        /// </summary>
        /// <param name="pair"></param>
        public static implicit operator SerializableKeyValuePair(KeyValuePair<string, string> pair)
        {
            return new SerializableKeyValuePair() { Key = pair.Key, Value = pair.Value };
        }
        /// <summary>
        /// Fancy exhibition on debug
        /// </summary>
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}