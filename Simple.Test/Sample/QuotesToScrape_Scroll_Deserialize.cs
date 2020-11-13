using System;
using RafaelEstevam.Simple.Spider.Interfaces;
using RafaelEstevam.Simple.Spider.Parsers;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class QuotesToScrape_Scroll_Deserialize
    {
        public static void run()
        {
            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
            // create a json parser for our QuotesObject class
            spider.Parsers.Add(new JsonDeserializeParser<QuotesObject>(parsedResult_event));
            // add first page /api/quotes?page={pageNo}
            spider.AddPage(buildPageUri(1), spider.BaseUri);
            // execute
            spider.Execute();
        }

        private static void parsedResult_event(object sender, ParserEventArgs<QuotesObject> args)
        {
            // add next
            if (args.ParsedData.has_next)
            {
                int next = args.ParsedData.page + 1;
                (sender as SimpleSpider).AddPage(buildPageUri(next), args.FetchInfo.Link);
            }
            // process data (show on console)
            foreach (var q in args.ParsedData.quotes)
            {
                Console.WriteLine($"{q.author.name }: { q.text }");
            }
        }
        public static Uri buildPageUri(int page)
        {
            string url = $"http://quotes.toscrape.com/api/quotes?page={ page }";
            return new Uri(url);
        }
    }
}