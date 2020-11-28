using Newtonsoft.Json.Linq;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Parsers
{
    /// <summary>
    ///  Generic Json parser, return Newtonsoft's JObject
    /// </summary>
    public class JsonParser : IParser<JObject>
    {
        /// <summary>
        /// Mime types supported by this parser
        /// </summary>
        public string[] MimeTypes => new string[] { "application/json" };
        /// <summary>
        /// Event with parsed data
        /// </summary>
        public event ParseData<JObject> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<JObject>(FetchInfo: FetchInfo, Data: JObject.Parse(FetchInfo.Html)));
        }
    }
}
