using System.Text;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Parsers
{
    public class XmlXElementParser : IParser<XElement>
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public string[] MimeTypes => new string[] { "application/xml", "text/html" };

        public event ParseData<XElement> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<XElement>(FetchInfo, XElement.Parse(Encoding.GetString(FetchInfo.Result))));
        }
    }
}
