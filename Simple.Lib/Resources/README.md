See full project at https://github.com/RafaelEstevamReis/SimpleSpider

# SimpleSpider

A simple and modular web spider written in C# .Net Core

Some advantages
* Very simple to use and operate, ideal for lots of small projects or personal ones
* Easy html filter with HObject (a HtmlNode wrap with use similar to JObject)
* Internal conversion from html to XElement, no need to external tools on use
* Automatic Json parser to JObject
* Automatic Json deserialize <T>
* Modular Parser engine (you can add your own parsers!)
  * JSON and XML already included
* Modular Caching engine (you can add your own!)
  * Stand alone Cache engine included, no need to external softwares
* Modular Downloader engine (you can add your own!)
  * WebClient with cookies or HttpClient download engine included

## Samples

Inside the [Simple.Tests](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Test/Sample) folders are various samples, these are some of them:

### Use Json to deserialize Quotes

Json response? Get a event with your data already deserialized.

( yes, these few lines below are full functional examples! )

```C#
void run()
{
    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
    // create a json parser for our QuotesObject class
    spider.Parsers.Add(new JsonDeserializeParser<QuotesObject>(parsedResult_event));
    // add first page /api/quotes?page={pageNo}
    spider.AddPage(buildPageUri(1), spider.BaseUri);
    // execute
    spider.Execute();
}
void parsedResult_event(object sender, ParserEventArgs<QuotesObject> args)
{
    // add next
    if (args.ParsedData.has_next)
    {
        int next = args.ParsedData.page + 1;
        (sender as SimpleSpider).AddPage(buildPageUri(next), args.FetchInfo.Link);
    }
    // process data (show on console)
    foreach (var q in args.ParsedData.quotes)
    {
        Console.WriteLine($"{q.author.name }: { q.text }");
    }
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_Scroll_Deserialize.cs)*

### Use XPath to select content

Use XPath to select html elements and filter data.

```C#
void run()
{
    var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
    // callback to gather items, new links are collected automatically
    spider.FetchCompleted += fetchCompleted_items;
    // Ignore (cancel) the pages containing "/reviews/" 
    spider.ShouldFetch += (s, a) => { a.Cancel = a.Link.Uri.ToString().Contains("/reviews/"); };
    
    // execute from first page
    spider.Execute();
}
void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
{
    // ignore all pages except the catalogue
    if (!args.Link.ToString().Contains("/catalogue/")) return;

    var XElement = HtmlToXElement.Parse(args.Html);
    // collect book data
    var articleProd = XElement.XPathSelectElement("//article[@class=\"product_page\"]");
    if (articleProd == null) return; // not a book
    // Book info
    string sTitle = articleProd.XPathSelectElement("//h1").Value;
    string sPrice = articleProd.XPathSelectElement("//p[@class=\"price_color\"]").Value;
    string sStock = articleProd.XPathSelectElement("//p[@class=\"instock availability\"]").Value.Trim();
    string sDesc = articleProd.XPathSelectElement("p")?.Value; // books can be description less
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/BooksToScrape.cs)*


### Easy single resource fetch

Easy API pooling for updates with single resource fetch.
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


### Use our HObject implementation to select content

Use indexing style object representation of the html document similar to Newtonsoft's JObject.

```C#
 void run()
{
    // Get Quotes.ToScrape.com as HObject
    HObject hObj = FetchHelper.FetchResourceHObject(new Uri("http://quotes.toscrape.com/"));
    ...
    // Example 2
    // Get all Spans and filter by Class='text'
    HObject ex2 = hObj["span"].OfClass("text");
    ...
    // Example 4
    // Get all Spans filters by some arbitrary attribute
    //  Original HTML: <span class="text" itemprop="text">
    HObject ex4 = hObj["span"].OfWhich("itemprop", "text");
    ...
    //Example 9
    // Exports Values as Strings with Method and implicitly
    string[] ex9A = hObj["span"].OfClass("text").GetValues();
    string[] ex9B = hObj["span"].OfClass("text");
    ...
    //Example 13
    // Gets Attribute's value
    string ex13 = hObj["footer"].GetClassValue();

    //Example 14
    // Chain query to specify item and then get Attribute Values
    // Gets Next Page Url
    string ex14A = hObj["nav"]["ul"]["li"]["a"].GetAttributeValue("href"); // Specify attribute
    string ex14B = hObj["nav"]["ul"]["li"]["a"].GetHrefValue();
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_HObject.cs)*


### Easy initialization with chaining

Initialize your spider easily with chaining and a good variety of options.

```C#
void run()
{
    var init = new InitializationParams()
        .SetCacher(new ContentCacher()) // Easy cache engine change
        .SetDownloader(new WebClientDownloader()) // Easy download engine change
        .SetSpiderStartupDirectory(@"D:\spiders\") // Default directory
        // create a json parser for our QuotesObject class
        .AddParser(new JsonDeserializeParser<QuotesObject>(parsedResult_event))
        .SetConfig(c => c.Enable_Caching()  // Already enabled by default
                         .Disable_Cookies() // Already disabled by default
                         .Disable_AutoAnchorsLinks()
                         .Set_CachingNoLimit() // Already set by default
                         .Set_DownloadDelay(5000));

    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"), init);

    // add first 
    spider.AddPage(buildPageUri(1), spider.BaseUri);
    // execute
    spider.Execute();
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_Chaining.cs)*

## Some Helpers
* FetchHelper: Fast single resource fetch with lots of parsers
* RequestHelper: Make requests (gets and posts) easily
* XmlSerializerHelper: Generic class to serialize and deserialize stuff using Xml, easy way to save what you collected without any database
* CSV Helper: Read csv files (even compressed) without external libraries
* UriHelper: Manipulates parts of the Uri

## Giants' shoulders
* Html parsing with Html Agility Pack
* Json parsing with Newtonsoft
* Logging with Serilog

Readme.md | Commit b4968d3 from 2020-11-13
