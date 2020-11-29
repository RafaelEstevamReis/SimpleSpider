using System;
using RafaelEstevam.Simple.Spider.Extensions;
using RafaelEstevam.Simple.Spider.Test.Sample;

namespace RafaelEstevam.Simple.Spider.Test.ModulesSamples
{
    public class Storage_Sqlite_Quotes
    {
        // Similar to [RafaelEstevam.Simple.Spider.Test.Sample.BooksToScrape],
        //   see for more in depth cover of the crawling part
        public static void run()
        {
            // Creates a new instance
            var storage = new Storage.SQLiteStorage<Quote>();
            // set the spider to use it
            var init = new InitializationParams()
                        .SetStorage(storage);

            var spider = new SimpleSpider("QuotesToScrape",
                                   new Uri("http://quotes.toscrape.com/"),
                                          init);

            Console.WriteLine($"The sqlite database is at {storage.DatabaseFilePath}");
            Console.WriteLine($"The quotes are being stored in the table {storage.TableNameOfT}");

            spider.FetchCompleted += spider_FetchCompleted;
            spider.ShouldFetch += Spider_ShouldFetch;
            spider.Execute();

            Console.WriteLine("Quotes from Albert Einstein");
            foreach (Quote q in storage.GetItemsWith("Author", "Albert Einstein"))
            {
                Console.WriteLine($"{q.Author}: {q.Text}");
            }
            Console.WriteLine("All Quotes");
            foreach (Quote q in spider.Storage.RetrieveAllItems())
            {
                Console.WriteLine($"{q.Author}: {q.Text}");
            }
        }

        private static void Spider_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            args.CancelIfContains("/login")
                .CancelIfContains("/tag");
        }

        private static void spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var hObj = args.GetHObject();

            var divQuotes = hObj["div > .quote"];
            foreach (var q in divQuotes)
            {
                var quote = new Quote()
                {
                    Text = q["span > .text"].GetValue().HtmlDecode(),
                    Author = q["small > .author"].GetValue().HtmlDecode(),
                    Tags = string.Join(';', q["a > .tag"].GetValues())
                };

                ((SimpleSpider)Sender).Storage.AddItem(args.Link, quote);
            }
        }
    }
}
