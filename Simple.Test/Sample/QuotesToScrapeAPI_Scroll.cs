using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using RafaelEstevam.Simple.Spider.Parsers;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class QuotesToScrapeAPI_Scroll
    {
        public static void run()
        {
            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
            // add callback to json pages
            spider.Parsers.OfType<JsonParser>().First().ParsedData += json_ParsedData;
            // add first
            spider.AddPage(buildPageUri(1), spider.BaseUri);
            // execute
            spider.Execute();
        }

        private static void json_ParsedData(object sender, Interfaces.ParserEventArgs<JObject> args)
        {
            // add next
            if ((bool)args.ParsedData["has_next"])
            {
                int currPage = (int)args.ParsedData["page"];
                ((SimpleSpider)sender).AddPage(buildPageUri(currPage + 1), args.FetchInfo.Link);
            }
            // process data (show on console)
            foreach (var j in args.ParsedData["quotes"])
            {
                Console.WriteLine($"{ (string)j["author"]["name"] }: { (string)j["text"] }");
            }
        }
        public static Uri buildPageUri(int page)
        {
            string url = $"http://quotes.toscrape.com/api/quotes?page={ page }";
            return new Uri(url);
        }
    }
}
