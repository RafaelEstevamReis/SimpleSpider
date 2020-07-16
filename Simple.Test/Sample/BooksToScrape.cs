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
            // callback to gather links
            spider.FetchCompleted += (s, a) =>
            {
                // Use a simple SubString-based split to get all "<a>" tags
                var links = Helper.AnchorHelper.GetAnchors(a.Link.Uri, a.Html);
                // Add the collected links to the queue
                (s as SimpleSpider).AddPage(links, a.Link);
            };
            // callback to gather items
            spider.FetchCompleted += fetchCompleted_items;
            // Ignore (cancel) the pages containing "/reviews/" 
            spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };
            
            // execute from first page
            spider.Execute();

            // List all books
            foreach (var b in lstBooks)
            {
                Console.WriteLine($"{b.Price:C2} {b.Title}");
            }
        }
        private static void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;

            // collect book data
            var articleProd = args.XElement.XPathSelectElement("//article[@class=\"product_page\"]");
            if (articleProd == null) return; // not a book
            // Book info
            string sTitle = articleProd.XPathSelectElement("//h1").Value;
            string sPrice = articleProd.XPathSelectElement("//p[@class=\"price_color\"]").Value;
            string sStock = articleProd.XPathSelectElement("//p[@class=\"instock availability\"]").Value.Trim();
            string sDesc = articleProd.XPathSelectElement("p")?.Value; // books can be descriptionless
            // convert price to Decimal
            decimal.TryParse(sPrice, NumberStyles.Currency, new CultureInfo("en-GB", false), out decimal price);
            
            lstBooks.Add(new BookData()
            {
                Title = sTitle,
                Price = price,
                Description = sDesc,
                StockInfo = sStock,
                PrductInfoTable = args.XElement.GetAllTables().First(),
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
