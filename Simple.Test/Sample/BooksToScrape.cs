using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using Net.RafaelEstevam.Spider.Extensions;
using System.Data;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    /// <summary>
    /// Simple sipider to crawl http://books.toscrape.com/
    /// </summary>
    public class BooksToScrape
    {
        static List<BookData> lstBooks = new List<BookData>();
        public static void run()
        {
            var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
            // callbacks
            spider.FetchCompleted += fetchCompleted_links; // callback to gather links
            spider.FetchCompleted += fetchCompleted_items; // callback to gather items
            spider.ShouldFetch += Spider_ShouldFetch;      // Ignore some pages
            // execute from first page
            spider.Execute();

            // List all books
            foreach (var b in lstBooks)
            {
                Console.WriteLine($"{b.Price:C2} {b.Title} ");
            }
        }

        private static void Spider_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            // All reviews links returns 404-Not Found, ignore them
            if (args.Link.Uri.ToString().Contains("/reviews/")) args.Cancel = true;
        }
        private static void fetchCompleted_links(object Sender, FetchCompleteEventArgs args)
        {
            // Use a simple SubString-based split to get all "<a>" tags
            var links = Helper.AnchorHelper.GetAnchors(args.Link.Uri, args.Html).ToArray();
            // Add the collected links to the queue
            (Sender as SimpleSpider).AddPage(links, args.Link);
        }
        private static void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;

            // collect book data
            var articleProd = args.XElement.XPathSelectElement("//article[@class=\"product_page\"]");
            if (articleProd == null) return; // not a book
            // Book info
            var sTitle = articleProd.XPathSelectElement("//h1").Value;
            var sPrice = articleProd.XPathSelectElement("//p[@class=\"price_color\"]").Value;
            var sStock = articleProd.XPathSelectElement("//p[@class=\"instock availability\"]").Value.Trim();
            var sDesc = articleProd.XPathSelectElement("p")?.Value; // books can be descriptonless
            // convert price to Decimal
            decimal.TryParse(sPrice, NumberStyles.Currency, new CultureInfo("en-GB", false), out decimal price);
            // Product info table
            var table = args.XElement.GetAllTables().First();
            
            lstBooks.Add(new BookData()
            {
                Title = sTitle,
                Price = price,
                Description = sDesc,
                StockInfo = sStock,
                PrductInfoTable = table,
            });
        }

        class BookData
        {
            public string Title { get; internal set; }
            public decimal Price { get; internal set; }
            public string Description { get; internal set; }
            public string StockInfo { get; internal set; }
            public DataTable PrductInfoTable { get; internal set; }
        }
    }
}
