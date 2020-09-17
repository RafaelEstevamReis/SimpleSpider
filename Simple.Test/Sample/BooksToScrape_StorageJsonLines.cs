using System;
using Net.RafaelEstevam.Spider.Wrappers.HTML;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    public class BooksToScrape_StorageJsonLines
    {
        public static void run()
        {
            var iP = new InitializationParams()
                // Defines Storage Engine
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
                var name = book.SelectTag("//h1").InnerText;
                var priceP = book.SelectTag<Paragraph>(".//p[@class=\"price_color\"]");
                var price = priceP.InnerText.Trim();

                (Sender as SimpleSpider).Storage.AddItem(args.Link, new { Name = name, Proce = price });
            }
        }
    }
}
