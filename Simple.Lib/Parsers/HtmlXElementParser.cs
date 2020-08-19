using System;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Parsers
{
    [Obsolete("Slow execution")]
    /// <summary>
    /// Html to XElement parser
    /// </summary>
    public class HtmlXElementParser : IParser<XElement>
    {
        /// <summary>
        /// MimeTypes supported by this parser
        /// </summary>
        public string[] MimeTypes => new string[] { "text/html" };
        
        /// <summary>
        /// Event with the html parsed as XElement
        /// </summary>
        public event ParseData<XElement> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<XElement>(FetchInfo: FetchInfo, Data: HtmlToXElement.Parse(FetchInfo.Html)));
        }
    }
}
