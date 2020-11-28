using System;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Test.ModulesSamples
{
    public class Storage_Sqlite_Quotes
    {
        // Similar to [RafaelEstevam.Simple.Spider.Test.Sample.BooksToScrape],
        //   see for more in depth cover of the crawling part
        public static void run()
        {
            var init = new InitializationParams()
                        .SetStorage(new Storage.SQLiteStorage<Quotes>());

            var spider = new SimpleSpider("QuotesToScrape",
                                   new Uri("http://quotes.toscrape.com/"), 
                                          init);
            spider.FetchCompleted += spider_FetchCompleted;
            spider.Execute();

            foreach (Quotes q in spider.Storage.RetrieveAllItems())
            {
                Console.WriteLine($"{q.Author}: {q.Text}");
            }
        }
        private static void spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var hObj = args.GetHObject();

            var divQuotes = hObj["div > .quote"];
            foreach (var q in divQuotes)
            {
                var quote = new Quotes()
                {
                    Author = q["small > .author"].GetValue(),
                    Text = q["span > .text"].GetValue(),
                    Tags = string.Join(';', q["a > .tag"].Select(o => o.GetValue()))
                };

                ((SimpleSpider)Sender).Storage.AddItem(args.Link, quote);
            }                
        }

        public class Quotes 
        {
            public string Author { get; set; }
            public string Text { get; set; }
            public string Tags { get; set; }
        }
    }
}
