using System;
using System.Collections.Generic;
using System.Text;
using Net.RafaelEstevam.Spider.Interfaces;
using Newtonsoft.Json;

namespace Net.RafaelEstevam.Spider.Parsers
{
    /// <summary>
    /// Json parser to deserialize given T class
    /// </summary>
    /// <typeparam name="T">Type to be deserialized to</typeparam>
    public class JsonDeserializeParser<T> : IParser<T> where T : new()
    {
        /// <summary>
        /// Empty/Default constructor
        /// </summary>
        public JsonDeserializeParser() { }
        /// <summary>
        /// Constructor with pre-defined callback. Easy to create and add reference in one line
        /// </summary>
        /// <param name="callback">Callback to be fired when this parser complete</param>
        public JsonDeserializeParser(ParseData<T> callback)
        {
            ParsedData = callback;
        }
        /// <summary>
        /// Mime types supported by this parser
        /// </summary>
        public string[] MimeTypes => new string[] { "application/json" };
        /// <summary>
        /// Event with parsed data
        /// </summary>
        public event ParseData<T> ParsedData;
        void IParserBase.Parse(SimpleSpider spider, FetchCompleteEventArgs fetchInfo)
        {
            var result = JsonConvert.DeserializeObject<T>(fetchInfo.Html);
            ParsedData(spider, new ParserEventArgs<T>(fetchInfo, result));
        }
    }
}