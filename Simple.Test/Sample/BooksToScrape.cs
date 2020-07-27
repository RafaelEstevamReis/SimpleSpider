using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Net.RafaelEstevam.Spider.Extensions;
using Net.RafaelEstevam.Spider.Helper;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    /// <summary>
    /// Simple sipider to crawl http://books.toscrape.com/
    /// </summary>
    public class BooksToScrape
    {
        public static void run()
        {
            var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
            // callback to gather links
            spider.FetchCompleted += (s, a) =>
            {
                // This callback can be replaced by:
                //  spider.Configuration.Auto_AnchorsLinks = true;
                // and is here for demonstration purposes

                // Use a simple SubString-based split to get all "<a>" tags
                var links = AnchorHelper.GetAnchors(a.Link.Uri, a.Html);
                // Add the collected links to the queue
                (s as SimpleSpider).AddPages(links, a.Link);
            };
            // callback to gather items
            spider.FetchCompleted += fetchCompleted_items;
            // Ignore (cancel) the pages containing "/reviews/" 
            spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };
            
            // execute from first page
            spider.Execute();

            // List all books
            foreach (var b in spider.CollectedItems())
            {
                Console.WriteLine($"{b.CollectAt:g} {b.CollectedOn}");
                Console.WriteLine($" > {((BookData)b.Object).Price:C2} {((BookData)b.Object).Title}");
            }
        }
        private static void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
        {
            // ignore all pages except the catalogue
            if (!args.Link.ToString().Contains("/catalogue/")) return;

            var XElement = HtmlToEXelement.Parse(args.Html);
            // collect book data
            var articleProd = XElement.XPathSelectElement("//article[@class=\"product_page\"]");
            if (articleProd == null) return; // not a book
            // Book info
            string sTitle = articleProd.XPathSelectElement("//h1").Value;
            string sPrice = articleProd.XPathSelectElement("//p[@class=\"price_color\"]").Value;
            string sStock = articleProd.XPathSelectElement("//p[@class=\"instock availability\"]").Value.Trim();
            string sDesc = articleProd.XPathSelectElement("p")?.Value; // books can be descriptionless
            // convert price to Decimal
            decimal.TryParse(sPrice, NumberStyles.Currency, new CultureInfo("en-GB", false), out decimal price);

            (Sender as SimpleSpider).Collect(new BookData()
            {
                Title = sTitle,
                Price = price,
                Description = sDesc,
                StockInfo = sStock,
                PrductInfoTable = XElement.GetAllTables().First(),
            }, args.Link);
        }

        public class BookData
        {
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string StockInfo { get; set; }
            public DataTable PrductInfoTable { get; set; }
        }
    }
}
