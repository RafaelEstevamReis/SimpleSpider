# SimpleSpider

A simple and modular web spider writen in C# .Net Core

Some advantages
* Very simple to use and operate, ideal to personal or one of projects
* Internal conversion from html to XElement, no need to external tools on use
* Automatic Json parser to JObject
* Automatic Json deserialize \<T>
* Modular Parser engine (you can add your own parsers!)
* Modular Caching engine (you can add your own!)
* Modular Downloader engine (you can add your own!)

Easy **import with [NuGet](https://www.nuget.org/packages/Net.RafaelEstevam.Spider.Simple.Lib)**

## Samples

Inside the [Simple.Tests](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Test/Sample) folders are various samples, these are some of them:

### Use Json to parse Quotes

Json response? Get a event with your data already deserialized

( yes, these few lines below are full functional exemples! )

```C#
void run()
{
    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
    // create a json parser for our QuotesObject class
    spider.Parsers.Add(new JsonDeserializeParser<QuotesObject>(parsedResult_event))
    // add first
    spider.AddPage(buildPageUri(1), spider.BaseUri);
    // execute
    spider.Execute();
}
void parsedResult_event(object sender, ParserEventArgs<QuotesObject> args)
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
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_Scroll_Deserialize.cs)*

### Use XPath to select content

Use XPath to select elements and filter data

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
    (Sender as SimpleSpider).AddPages(AnchorHelper.GetAnchors(args.Link.Uri, args.Html), args.Link);

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
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/BooksToScrape.cs)*


### Easy initialization with chaining

Initialzie your spider easly with chaining 

```C#
void run()
{
    var init = new InitializationParams()
        .SetCacher(new ContentCacher())
        .SetDownloader(new WebClientDownloader())
        .SetSpiderStarupDirectory(@"D:\spiders\") // Default directory
        // create a json parser for our QuotesObject class
        .AddParser(new JsonDeserializeParser<QuotesObject>(parsedResult_event))
        .SetConfig(c => c.Enable_Caching()
                         .Disable_Cookies()
                         .Disable_AutoAnchorsLinks()
                         .Set_CachingNoLimit()
                         .Set_DownloadDelay(5000));

    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"), init);

    // add first
    spider.AddPage(buildPageUri(1), spider.BaseUri);
    // execute
    spider.Execute();
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_Chaining.cs)*


### Easy single resource fetch

Easy API pooling for updates
```C#
void run()
{
    var uri = new Uri("http://quotes.toscrape.com/api/quotes?page=1");
    var quotes = FetchHelper.FetchResourceJson<QuotesObject>(uri);
    // show the quotes deserialized
    foreach (var quote in quotes.quotes)
    {
        Console.WriteLine($"Quote: {quote.text}");
        Console.WriteLine($"       - {quote.author.name}");
        Console.WriteLine();
    }
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/ApiPooler_FetcherHelper.cs)*

## Some [Helpers](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Lib/Helper)
* [FormsHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/FormsHelper.cs): Deserialize html forms to easy manipulate data and create new requests
* [XmlSerializerHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/XmlSerializerHelper.cs): Generic class to serialize and deserialzie stuff using Xml, easy way to save what you collected without any database
* [CSV Helper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/CSVHelper.cs): Read csv files even compressed without exernal libraries
* XElement to Stuff: Extract [tables](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/XElementHelper.cs#L17) from page in DataTable

## Giants' shoulders
* Html parsing with [Html Agility Pack](https://html-agility-pack.net/)
* Json parsing with [Newtonsoft](https://www.newtonsoft.com/json)
* Logging with [Serilog](https://serilog.net/)



Readme.md | Commit 7851975 from 2020-07-24
