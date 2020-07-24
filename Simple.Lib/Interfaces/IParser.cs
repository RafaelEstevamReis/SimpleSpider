using System;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    public delegate void ParseData<T>(object sender, ParserEventArgs<T> parserEventArgs);

    public interface IParserBase
    {
        string[] MimeTypes { get; }
        internal void Parse(SimpleSpider spider, FetchCompleteEventArgs fetchInfo);
    }
    public interface IParser<T> : IParserBase
    {
        event ParseData<T> ParsedData;
    }
    public class ParserEventArgs<T> : EventArgs
    {
        public ParserEventArgs(FetchCompleteEventArgs FetchInfo, T Data)
        {
            this.FetchInfo = FetchInfo;
            this.ParsedData = Data;
        }

        public FetchCompleteEventArgs FetchInfo { get; }
        public T ParsedData { get; }
    }
}
