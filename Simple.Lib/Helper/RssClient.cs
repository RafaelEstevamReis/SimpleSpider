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
        }
        public class rssChannelItem
        {
            public rssEnclosure enclosure { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string creator { get; set; }
            public string credit { get; set; }
            public string pubDate { get; set; }
            public string author { get; set; }
            public string link { get; set; }
            public string guid { get; set; }

            [XmlElementAttribute("category")]
            public rssCategory[] categories { get; set; }
        }
        public class rssEnclosure
        {
            [XmlAttribute()]
            public string url { get; set; }
            [XmlAttribute()]
            public string length { get; set; }
            [XmlAttribute("type")]
            public string mimeType { get; set; }
        }
        public class rssCategory
        {

            [System.Xml.Serialization.XmlAttribute()]
            public string domain { get; set; }

            [System.Xml.Serialization.XmlText()]
            public string Value { get; set; }
        }
    }
}
