using System.Text;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Parsers
{
    /// <summary>
    /// Generic Xml to XElement parser
    /// </summary>
    public class XmlXElementParser : IParser<XElement>
    {
        /// <summary>
        /// Encoding to be used by the parser
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// Mime types supported by this parser
        /// </summary>
        public string[] MimeTypes => new string[] { "application/xml", "text/html" };
        /// <summary>
        /// Event with parsed data
        /// </summary>
        public event ParseData<XElement> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<XElement>(FetchInfo, XElement.Parse(Encoding.GetString(FetchInfo.Result))));
        }
    }
}
