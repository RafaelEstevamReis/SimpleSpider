using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    /// <summary>
    /// Simple sipider to crawl http://books.toscrape.com/
    /// </summary>
    public class BooksToScrape
    {
        public static void run()
        {
            // Spider initialization
            var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
            // callbacks
            spider.FetchCompleted += Spider_FetchCompleted;
            spider.ShouldFetch += Spider_ShouldFetch;
            // configuration
            spider.Configuration.DownloadDelay = 5000; // be kind to others
            // add first page
            spider.AddPage(spider.BaseUri, spider.BaseUri);
            // execute
            spider.Execute();
        }
        private static void Spider_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            // All reviews links returns 404-Not Found
            // ignore, them
            if (args.Link.Uri.ToString().Contains("/reviews/")) args.Cancel = true;
        }
        private static void Spider_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            // Use a simple SubString-based split to get all "<a>" tags
            var links = Helper.AnchorHelper.GetAnchors(args.Link.Uri, args.Html).ToArray();
            // Add the collected links to the queue
            (Sender as SimpleSpider).AddPage(links, args.Link);

            // Extract data:
            // ...
        }
    }
}
