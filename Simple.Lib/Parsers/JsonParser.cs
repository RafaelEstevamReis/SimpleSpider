using Net.RafaelEstevam.Spider.Interfaces;
using Newtonsoft.Json.Linq;

namespace Net.RafaelEstevam.Spider.Parsers
{
    public class JsonParser : IParser<JObject>
    {
        public string[] MimeTypes => new string[] { "application/json" };

        public event ParseData<JObject> ParsedData;

        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs FetchInfo)
        {
            if (ParsedData == null) return;

            ParsedData(spider, new ParserEventArgs<JObject>(FetchInfo: FetchInfo, Data: JObject.Parse(FetchInfo.Html)));
        }
    }
}
