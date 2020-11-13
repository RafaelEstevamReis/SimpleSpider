using System;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class BooksToScrape_StorageJsonLines
    {
        public static void run()
        {
            var iP = new InitializationParams()
                // Defines a Storage Engine
                // All stored items will be in spider folder as JsonLines
                .SetStorage(new Storage.JsonLinesStorage()); // JsonLines: https://jsonlines.org/

            var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"), iP);
            // callback to gather items
            spider.FetchCompleted += fetchCompleted_items;
            // Ignore (cancel) the pages containing "/reviews/" 
            spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };
            // execute from first page
            spider.Execute();
        }

        private static void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;

            var tag = new Tag(args.GetDocument());
            var books = tag.SelectTags<Article>("//article[@class=\"product_page\"]");

            foreach (var book in books)
            {
                var priceP = book.SelectTag<Paragraph>(".//p[@class=\"price_color\"]");
                var price = priceP.InnerText.Trim();

                (Sender as SimpleSpider).Storage.AddItem(args.Link, new
                {
                    name = book.SelectTag("//h1").InnerText,
                    price
                });
            }
        }
    }
}
