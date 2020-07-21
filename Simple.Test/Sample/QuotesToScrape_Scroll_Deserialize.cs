using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    public class QuotesToScrape_Scroll_Deserialize
    {
        public static void run()
        {
            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
            // createa json parser for our QuotesObject class
            var parser = new Parsers.JsonDeserializeParser<QuotesObject>(parsedResult_event);
            spider.Parsers.Add(parser);

            // add first
            spider.AddPage( buildPageUri(1), spider.BaseUri);
            // execute
            spider.Execute();
        }

        private static void parsedResult_event(object sender, Interfaces.ParserEventArgs<QuotesObject> args)
        {
            // add next
            if (args.ParsedData.has_next)
            {
                int currPage = args.ParsedData.page;
                ((SimpleSpider)sender).AddPage(buildPageUri(currPage + 1), args.FetchInfo.Link);
            }
            // process data (show on console)
            foreach (var j in args.ParsedData.quotes)
            {
                Console.WriteLine($"{j.author.name }: { j.text }");
            }
        }
        public static Uri buildPageUri(int page)
        {
            string url = $"http://quotes.toscrape.com/api/quotes?page={ page }";
            return new Uri(url);
        }

        public class QuotesObject
        {
            public bool has_next { get; set; }
            public int page { get; set; }

            public Quote[] quotes { get; set; }
        }
        public class Quote
        {
            public Author author { get; set; }
            public string[] tags { get; set; }
            public string text { get; set; }
        }
        public class Author
        {
            public string goodreads_link { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }
    }
}
