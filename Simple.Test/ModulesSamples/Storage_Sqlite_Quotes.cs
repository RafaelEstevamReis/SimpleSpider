using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafaelEstevam.Simple.Spider.Test.ModulesSamples
{
    public class Storage_Sqlite_Quotes
    {
        // Similar to [RafaelEstevam.Simple.Spider.Test.Sample.BooksToScrape],
        //   see for more in depth cover of the crawling part
        public static void run()
        {
            var sqliteStorage = new Storage.SQLiteStorage<Quotes>();

            var init = new InitializationParams()
                        .SetStorage(sqliteStorage);

            var spider = new SimpleSpider("QuotesToScrape",
                                   new Uri("http://quotes.toscrape.com/"), 
                                          init);
            spider.FetchCompleted += spider_FetchCompleted;
            spider.Execute();
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
