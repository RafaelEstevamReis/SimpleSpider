using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RafaelEstevam.Simple.Spider.Helper
{
    public class AtomClient
    {
        private HttpClient client;

        public AtomClient()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
        }

        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<FeedModel> GetAsync(string url)
            => await GetAsync(new Uri(url));
        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<FeedModel> GetAsync(Uri uri)
        {
            var msg = await client.GetAsync(uri);
            msg.EnsureSuccessStatusCode();

            var xml = await msg.Content.ReadAsStringAsync();
            return Parse(xml);
        }

        /// <summary>
        /// Parse XML as RSS
        /// </summary>
        public static FeedModel Parse(string xml)
        {
            if (xml == null) return null;

            var serializer = new XmlSerializer(typeof(FeedModel));
            using var sr = new StringReader(xml);
            return (FeedModel)serializer.Deserialize(sr);
        }
        /// <summary>
        /// Fetch an Uri as XML and parse as RSS
        /// </summary>
        public static FeedModel Download(Uri uri, Encoding enc = null)
            => FetchHelper.FetchResourceXml<FeedModel>(uri, enc, false);
    }

    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public partial class FeedModel
    {
        public string id { get; set; }
        public feedTitle title { get; set; }
        public System.DateTime updated { get; set; }
        public feedLink link { get; set; }
        public feedAuthor author { get; set; }
        public string generator { get; set; }
        [XmlElement("category")]
        public feedCategory[] category { get; set; }
        public string logo { get; set; }
        public feedRights rights { get; set; }
        [XmlElement("entry")]
        public feedEntry[] entry { get; set; }


        public partial class feedTitle
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlText()]
            public string Value { get; set; }

            public override string ToString()
                => Value;
        }

        public partial class feedLink
        {
            [XmlAttribute()]
            public string rel { get; set; }
            [XmlAttribute()]
            public string href { get; set; }
            public override string ToString()
                => href;
        }
        public partial class feedAuthor
        {
            public string name { get; set; }
            public string email { get; set; }
            public string uri { get; set; }
            public override string ToString()
                => name;
        }
        public partial class feedCategory
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlAttribute()]
            public string term { get; set; }
            [XmlAttribute()]
            public string label { get; set; }

            public override string ToString()
                => label;
        }
        public partial class feedRights
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlText()]
            public string Value { get; set; }
            public override string ToString()
                => Value;
        }
        public partial class feedEntry
        {
            public string id { get; set; }

            [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
            public uint identifier { get; set; }
            public DateTime updated { get; set; }
            public DateTime published { get; set; }
            [XmlElement("category")]
            public feedEntryCategory[] category { get; set; }
            public string rights { get; set; }
            public feedEntryTitle title { get; set; }
            public feedEntrySummary summary { get; set; }
            [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
            public string subject { get; set; }
            [XmlElement("link")]
            public feedEntryLink[] link { get; set; }

            public override string ToString()
                => title?.Value ?? subject;
        }

        public partial class feedEntryCategory
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlAttribute()]
            public string term { get; set; }
            [XmlAttribute()]
            public string label { get; set; }

            public override string ToString()
                => label;
        }

        public partial class feedEntryTitle
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlText()]
            public string Value { get; set; }

            public override string ToString()
                => Value;
        }
        public partial class feedEntrySummary
        {
            [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang { get; set; }
            [XmlText()]
            public string Value { get; set; }
            public override string ToString()
                => Value;
        }

        public partial class feedEntryLink
        {
            [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
            public string title { get; set; }
            [XmlArray(Namespace = "http://search.yahoo.com/mrss/")]
            [XmlArrayItem("thumbnail", IsNullable = false)]
            public contentThumbnail[] content { get; set; }
            [XmlAttribute()]
            public string rel { get; set; }
            [XmlAttribute()]
            public string type { get; set; }
            [XmlAttribute("title")]
            public string titleAttribute { get; set; }
            [XmlAttribute()]
            public string href { get; set; }

            public override string ToString()
                => href;
        }
        public partial class contentThumbnail
        {
            [XmlElement(Namespace = "http://www.w3.org/2005/Atom")]
            public img img { get; set; }
            [XmlAttribute()]
            public string url { get; set; }
            [XmlAttribute()]
            public byte width { get; set; }
            [XmlAttribute()]
            public byte height { get; set; }

            public override string ToString()
                => url;
        }
        public partial class img
        {
            [XmlAttribute()]
            public string alt { get; set; }
            [XmlAttribute()]
            public byte width { get; set; }
            [XmlAttribute()]
            public byte height { get; set; }
            [XmlAttribute()]
            public string src { get; set; }
            public override string ToString()
                => src;
        }
        public partial class content
        {
            [XmlElement("thumbnail")]
            public contentThumbnail[] thumbnail { get; set; }
        }
    }
}
