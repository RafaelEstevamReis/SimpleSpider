using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// RSS client with cookies support
    /// </summary>
    public class RssClient
    {
        private HttpClient client;
        /// <summary>
        /// Transforms current XML before deserializing
        /// </summary>
        public Func<string, string> TransformXML { get; set; } = null;

        public RssClient()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
        }

        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<RssModel> GetAsync(string url)
            => await GetAsync(new Uri(url));
        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<RssModel> GetAsync(Uri uri)
        {
            var msg = await client.GetAsync(uri);
            msg.EnsureSuccessStatusCode();

            var xml = await msg.Content.ReadAsStringAsync();
            if (TransformXML != null) xml = TransformXML(xml);
            return Parse(xml);
        }

        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<RssModel> GetAsync(string url, Encoding encoding)
            => await GetAsync(new Uri(url), encoding);
        /// <summary>
        /// Fetch an URI with cookies and parse as RSS
        /// </summary>
        public async Task<RssModel> GetAsync(Uri uri, Encoding encoding)
        {
            if (encoding is null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var msg = await client.GetAsync(uri);
            msg.EnsureSuccessStatusCode();

            var bytes = await msg.Content.ReadAsByteArrayAsync();
            var xml = encoding.GetString(bytes);
            if (TransformXML != null) xml = TransformXML(xml);
            return Parse(xml);
        }

        /// <summary>
        /// Parse XML as RSS
        /// </summary>
        public static RssModel Parse(string xml)
        {
            if (xml == null) return null;

            var serializer = new XmlSerializer(typeof(RssModel));
            using var sr = new StringReader(xml);
            return (RssModel)serializer.Deserialize(sr);
        }
        /// <summary>
        /// Fetch an Uri as XML and parse as RSS
        /// </summary>
        public static RssModel Download(Uri uri, Encoding enc = null)
            => FetchHelper.FetchResourceXml<RssModel>(uri, enc, false);
    }

    [XmlRoot("rss")]
    public class RssModel
    {
        public rssChannel channel { get; set; }
        [XmlAttribute()]
        public string version { get; set; }

        public class rssChannel
        {
            public string title { get; set; }
            public string link { get; set; }

            public string description { get; set; }
            public string language { get; set; }
            public string copyright { get; set; }
            public string lastBuildDate { get; set; }
            public string pubDate { get; set; }

            [XmlElement("item")]
            public rssChannelItem[] item { get; set; }

            public override string ToString()
                => title;
        }
        public class rssChannelItem
        {
            public rssEnclosure enclosure { get; set; }
            public string title { get; set; }
            [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
            public string subject { get; set; }
            public string description { get; set; }
            public string content { get; set; }

            [XmlElement("encoded", Namespace = "http://purl.org/rss/1.0/modules/content/")]
            public string encodedContent { get; set; }

            [XmlElement("content", Namespace = "http://search.yahoo.com/mrss/")]
            public MediaContent mediaContent { get; set; }

            public string creator { get; set; }
            public string credit { get; set; }
            public string pubDate { get; set; }
            [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/")]
            public string date { get; set; }
            public rssAuthor author { get; set; }
            public string link { get; set; }
            public string guid { get; set; }

            [XmlElement("category")]
            public rssCategory[] categories { get; set; }

            public override string ToString()
                => title;
        }
        public class rssAuthor
        {
            [XmlElement("name")]
            public string Name { get; set; }

            [XmlText]
            public string Value { get; set; }
        }
        public class rssEnclosure
        {
            [XmlAttribute()]
            public string url { get; set; }
            [XmlAttribute()]
            public string length { get; set; }
            [XmlAttribute("type")]
            public string mimeType { get; set; }

            public override string ToString()
                => url;
        }
        public class rssCategory
        {
            [XmlAttribute()]
            public string domain { get; set; }
            [XmlText()]
            public string Value { get; set; }

            public override string ToString()
                => Value;
        }

        [XmlRoot(Namespace = "http://search.yahoo.com/mrss/", IsNullable = false)]
        public class MediaContent
        {
            public string title { get; set; }
            public string description { get; set; }
            [XmlAttribute()]
            public string url { get; set; }
            [XmlAttribute()]
            public string type { get; set; }
            [XmlAttribute()]
            public string expression { get; set; }
            [XmlAttribute()]
            public ushort width { get; set; }
            [XmlAttribute()]
            public ushort height { get; set; }

            public override string ToString()
                => title ?? url;
        }
    }

}
