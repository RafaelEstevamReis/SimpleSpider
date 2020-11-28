using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using RafaelEstevam.Simple.Spider.Helper;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    /// <summary>
    /// Simple spider to crawl http://books.toscrape.com/
    /// </summary>
    public class BooksToScrape
    {
        static List<BookData> items;
        public static void run()
        {
            items = new List<BookData>();
            var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
            // callback to gather links
            spider.FetchCompleted += (s, a) =>
            {
                // This callback can be replaced by:
                //  spider.Configuration.Auto_AnchorsLinks = true; (which is Enabled by default)
                // and is here for demonstration purposes

                // Use a simple SubString-based split to get all "<a>" tags
                var links = AnchorHelper.GetAnchors(a.Link.Uri, a.Html);
                // Add the collected links to the queue
                (s as SimpleSpider).AddPages(links, a.Link);
            };
            // callback to gather items
            spider.FetchCompleted += fetchCompleted_items_XPath;   // Sample using XPath
            spider.FetchCompleted += fetchCompleted_items_HObject; //Sample using HObject
            // Ignore (cancel) the pages containing "/reviews/" 
            spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };

            // execute from first page
            spider.Execute();

            // List all books
            foreach (var b in items)
            {
                Console.WriteLine($" > {b.Price:C2} {b.Title}");
            }
        }
        // Example using XPath
        private static void fetchCompleted_items_XPath(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;
            // HObject also processes XPath
            var hObj = args.GetHObject();
            // collect book data
            var articleProd = hObj.XPathSelect("//article[@class=\"product_page\"]");
            if (articleProd.IsEmpty()) return; // not a book
            // Book info
            string sTitle = articleProd.XPathSelect("//h1");
            string sPrice = articleProd.XPathSelect("//p[@class=\"price_color\"]");
            string sStock = articleProd.XPathSelect("//p[@class=\"instock availability\"]").GetValue().Trim();
            string sDesc = articleProd.XPathSelect("p")?.GetValue(); // books can be descriptionless
            // convert price to Decimal
            decimal.TryParse(sPrice, NumberStyles.Currency, new CultureInfo("en-GB", false), out decimal price);

            items.Add(new BookData()
            {
                Title = sTitle,
                Price = price,
                Description = sDesc,
                StockInfo = sStock,
            });
        }
        // Example using HObject
        private static void fetchCompleted_items_HObject(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;

            var hObj = args.GetHObject();
            // collect book data
            var articleProd = hObj["article > .product_page"]; // .XPathSelect("//article[@class=\"product_page\"]");
            if (articleProd.IsEmpty()) return; // not a book
            // Book info
            string sTitle = articleProd["h1"];  // .XPathSelect("//h1").Value;
            string sPrice = articleProd["p > .price_color"];// .XPathSelect("//p[@class=\"price_color\"]").Value;
            string sStock = articleProd["p > .instock"].GetValue().Trim();// .XPathSelect("//p[@class=\"instock\"]").Value.Trim();
            string sDesc = articleProd.Children("p");// .XPathSelect("p")?.Value; // books can be descriptionless
            // convert price to Decimal
            decimal.TryParse(sPrice, NumberStyles.Currency, new CultureInfo("en-GB", false), out decimal price);

            items.Add(new BookData()
            {
                Title = sTitle,
                Price = price,
                Description = sDesc,
                StockInfo = sStock,
            });
        }

        public class BookData
        {
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string StockInfo { get; set; }
            public DataTable ProductInfoTable { get; set; }
        }
    }
}
