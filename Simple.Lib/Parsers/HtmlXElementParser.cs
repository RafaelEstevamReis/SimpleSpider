using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Parsers
{
    public class HtmlXElementParser : IParser<XElement>
    {
        public string[] MimeTypes => new string[] { "text/html" };

        public event ParseData<XElement> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<XElement>(FetchInfo: FetchInfo, Data: HtmlToEXelement.Parse(FetchInfo.Html)));
        }
    }
}
