using RafaelEstevam.Simple.Spider.Interfaces;
using RafaelEstevam.Simple.Spider.Wrappers;

namespace RafaelEstevam.Simple.Spider.Parsers
{
    /// <summary>
    /// Html to HObject parser
    /// </summary>
    public class HObjectParser : IParser<HObject>
    {
        /// <summary>
        /// MimeTypes supported by this parser
        /// </summary>
        public string[] MimeTypes => new string[] { "text/html" };

        /// <summary>
        /// Event with the html parsed as HObject
        /// </summary>
        public event ParseData<HObject> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<HObject>(FetchInfo: FetchInfo, Data: FetchInfo.GetHObject()));
        }
    }
}
