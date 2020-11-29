using System;
using RafaelEstevam.Simple.Spider.Cachers;
using RafaelEstevam.Simple.Spider.Downloaders;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class QuotesToScrapeAPI_Chaining
    {
        public static void run()
        {
            var init = new InitializationParams()
                .SetCacher(new ContentCacher())
                .SetDownloader(new WebClientDownloader())
                // create a json parser for our QuotesObject class
                .AddParser(new Parsers.JsonDeserializeParser<QuotesObject>(parsedResult_event)) // Received Json class
                // Adds a SQLite storage to keep all collected quotes
                .SetStorage(new Storage.SQLiteStorage<Quote>()) // Single quote class
                .SetConfig(c => c.Enable_Caching()
                                 .Disable_Cookies()
                                 .Disable_AutoAnchorsLinks()
                                 .Set_CachingNoLimit()
                                 .Set_DownloadDelay(5000));

            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"), init);

            // add first
            spider.AddPage(buildPageUri(1), spider.BaseUri);
            // execute
            spider.Execute();
        }

        private static void parsedResult_event(object sender, Interfaces.ParserEventArgs<QuotesObject> args)
        {
            var spider = ((SimpleSpider)sender);
            // add next
            if (args.ParsedData.has_next)
            {
                int currPage = args.ParsedData.page;
                spider.AddPage(buildPageUri(currPage + 1), args.FetchInfo.Link);
            }

            // Storage data ...
            foreach (var q in args.ParsedData.quotes)
            {
                var quote = new Quote()
                {
                    Text = q.text,
                    Author = q.author.name,
                    Tags = string.Join(';', q.tags)
                };

                spider.Storage.AddItem(args.FetchInfo.Link, quote);
            }
        }
        public static Uri buildPageUri(int page)
        {
            string url = $"http://quotes.toscrape.com/api/quotes?page={ page }";
            return new Uri(url);
        }

    }
}
