using System;
using System.Collections.Generic;
using System.Text;
using Net.RafaelEstevam.Spider.Interfaces;
using Newtonsoft.Json;

namespace Net.RafaelEstevam.Spider.Parsers
{
    public class JsonDeserializeParser<T> : IParser<T> where T : new()
    {
        public JsonDeserializeParser() { }
        public JsonDeserializeParser(ParseData<T> callback)
        {
            ParsedData = callback;
        }

        public string[] MimeTypes => new string[] { "application/json" };

        public event ParseData<T> ParsedData;
        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs fetchInfo)
        {
            var result = JsonConvert.DeserializeObject<T>(fetchInfo.Html);
            ParsedData(spider, new ParserEventArgs<T>(fetchInfo, result));
        }
    }
}