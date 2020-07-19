# SimpleSpider

[!] Work in Progress

A simple and modular web spider writen in C# .Net Core

Some advantages
* Very simple to use and operate, ideal to personal or one of projects
* Internal conversion from html to XElement, no need to external tools on use
* Automatic Json parser to JObject
* Automatic Json deserialize <T>
* Modular Parser engine (you can add your own parsers!)
* Modular Caching engine (you can add your own!)
* Modular Downloader engine (you can add your own!)

## Samples

Inside Simple.Tests are some spiders to show to crawl and collect data

### Use Json to parse Quotes
[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_Scroll_Advanced.cs)
```C#
void run()
{
    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
    // createa json parser for our QuotesObject class
    spider.Parsers.Add(new Parsers.JsonDeserializeParser<QuotesObject>(parsedResult_event))
    // add first
    spider.AddPage( buildPageUri(1), spider.BaseUri);
    // execute
    spider.Execute();
}
void parsedResult_event(object sender, Interfaces.ParserEventArgs<QuotesObject> args)
{
    // add next
    if (args.ParsedData.has_next)
    {
        int currPage = args.ParsedData.page;
        ((SimpleSpider)sender).AddPage(buildPageUri(currPage + 1), args.FetchInfo.Link);
    }
    // process data (show on console)
    foreach (var j in args.ParsedData.quotes)
    {
        Console.WriteLine($"{j.author.name }: { j.text }");
    }
}
```

### Use XPath to select content
[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/BooksToScrape.cs)
```C#
void run()
{
    var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
    // callback to gather items
    spider.FetchCompleted += fetchCompleted_items;
    // Ignore (cancel) the pages containing "/reviews/" 
    spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };
    
    // execute from first page
    spider.Execute();
}
void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
{
    // Colect new links
    (Sender as SimpleSpider).AddPage(AnchorHelper.GetAnchors(args.Link.Uri, args.Html), args.Link);

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
}
```

## Some Helpers
* XmlSerializer helper: Generic class to serialize and deserialzie stuff using Xml, easy way to save what you collected without any database
* CSV helper: Read csv files even compressed without exernal libraries
* XElement to Stuff: Extract tables from page in DataTable

## Giant shoulders
* Html parsing with [Html Agility Pack](https://html-agility-pack.net/)
* Json parsing with [Newtonsoft](https://www.newtonsoft.com/json)
* Logging with [Serilog](https://serilog.net/)
