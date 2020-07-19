using System;
using System.Linq;
using Net.RafaelEstevam.Spider.Parsers;
using Newtonsoft.Json.Linq;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    public class QuotesToScrape_Scroll
    {
        public static void run()
        {
            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
            // add callback to json pages
            spider.Parsers.OfType<JsonParser>().First().ParsedData += json_ParsedData;
            // add first
            spider.AddPage(new Uri("http://quotes.toscrape.com/api/quotes?page=1"), spider.BaseUri);
            // execute
            spider.Execute();
        }

        private static void json_ParsedData(object sender, Interfaces.ParserEventArgs<JObject> args)
        {
            if ((bool)args.ParsedData["has_next"])
            {
                var url = args.FetchInfo.Link.ToString();
                var pageAtual = int.Parse(url.Split('=')[1]);
                ((SimpleSpider)sender).AddPage(new Uri($"{ url.Split('=')[0] }={ pageAtual + 1 }"), args.FetchInfo.Link);
            }

            foreach (var j in args.ParsedData["quotes"])
            {
                Console.WriteLine($"{ (string)j["author"]["name"] }: { (string)j["text"] }");
            }
        }
    }
}
