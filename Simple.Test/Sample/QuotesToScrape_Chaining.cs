using System;
using System.Linq;
using RafaelEstevam.Simple.Spider.Cachers;
using RafaelEstevam.Simple.Spider.Downloaders;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class QuotesToScrape_Chaining
    {
        public static void run()
        {
            var init = new InitializationParams()
                .SetCacher(new ContentCacher())
                .SetDownloader(new WebClientDownloader())
                //.SetOfflineMode() // Remove all downloaders (sets a NullDownloader)
                .SetConfig(c => c.Enable_Caching()
                                 .Disable_Cookies()
                                 .Enable_AutoAnchorsLinks() // enable automatic link following
                                 .Set_CachingNoLimit()
                                 .Set_DownloadDelay(5000));

            var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"), init);

            // Defines pages that should not be fetched
            spider.ShouldFetch += (_, args)
                // ShouldFetch args also supports chaining to easy cancel resources
                // Note: the order is very important
                => args.CancelIfContains("/login")
                       .CancelIfContains("/tag/") // not fetch tags
                       .AllowIfContains("/tag/choices/") // I like to have choices =)
                       ;

            // Sets up the fetch completed callback
            spider.FetchCompleted += Spider_FetchCompleted;
            // execute
            spider.Execute();
        }

        private static void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            var hObj = args.GetHObject();

            var divQuotes = hObj["div > .quote"];
            foreach (var q in divQuotes)
            {
                var Author = q["small > .author"].GetValue();
                var Text = q["span > .text"].GetValue();
                var Tags = string.Join(';', q["a > .tag"].GetValues());

                Console.WriteLine($"Author: {Author}: {Text}");
                Console.WriteLine($"    > {Tags}");
            }
        }
    }
}
