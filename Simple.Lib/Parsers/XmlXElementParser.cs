using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Parsers
{
    public class XmlXElementParser : IParser<XElement>
    {
        public string[] MimeTypes => new string[] { "application/xml", "text/html" };

        public event ParseData<XElement> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<XElement>(FetchInfo: FetchInfo, Data: XElement.Parse(FetchInfo.Html)));
        }
    }
}
