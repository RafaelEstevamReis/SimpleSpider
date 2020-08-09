using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Net.RafaelEstevam.Spider
{
    public sealed partial class SimpleSpider
    {
        /// <summary>
        /// Internal spider data storage
        /// </summary>
        public sealed class SpiderData
        {
            /// <summary>
            /// Constructor of the class
            /// </summary>
            public SpiderData()
            {
                Error404 = new HashSet<string>();
                Moved301 = new Dictionary<string, string>();
            }
            /// <summary>
            /// List of pages that once got a 404 error and should not be downloaded again
            /// </summary>
            public HashSet<string> Error404 { get; set; }

            [XmlIgnore]
            public Dictionary<string, string> Moved301 { get; private set; }

            /// <summary>
            /// DO NOT USE - Serializable version of internal dictionary
            /// </summary>
            public SerializableKeyValuePair[] _Redirect301
            {
                get
                {
                    return Moved301.Select(o => (SerializableKeyValuePair)o).ToArray();
                }
                set
                {
                    // ignores duplicate keys
                    foreach (var pair in value)
                    {
                        Moved301[pair.Key] = pair.Value;
                    }
                }
            }
        }
    }
}
