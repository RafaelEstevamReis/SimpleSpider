 **THIS BRANCH IS CHANGING THE NAMESPACE AND THE NuGet PACKAGE**

# Simple.Spider

A simple and modular web spider written in C#. Multi Target with:
* .Net 5.0
* .Net Core 3.1
* .NET Standard 2.1

[![The MIT License](https://img.shields.io/github/license/RafaelEstevamReis/SimpleSpider)](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/LICENSE)
[![NuGet](https://buildstats.info/nuget/RafaelEstevam.Simple.Spider)](https://www.nuget.org/packages/RafaelEstevam.Simple.Spider/)
![.NET Core](https://github.com/RafaelEstevamReis/SimpleSpider/workflows/.NET%20Core/badge.svg)

Old package: [![NuGet](https://buildstats.info/nuget/Net.RafaelEstevam.Spider.Simple.Lib)](http://nuget.org/packages/Net.RafaelEstevam.Spider.Simple.Lib)

# Content
<!-- TOC -->
- [Simple.Spider](#simplespider)
- [Content](#content)
  - [Some advantages](#some-advantages)
  - [Installation](#installation)
  - [Getting started](#getting-started)
  - [Samples](#samples)
    - [Use XPath to select content](#use-xpath-to-select-content)
    - [Use our HObject implementation to select content](#use-our-hobject-implementation-to-select-content)
    - [Easy storage with StorageEngines](#easy-storage-with-storageengines)
    - [Easy initialization with chaining](#easy-initialization-with-chaining)
    - [Easy single resource fetch](#easy-single-resource-fetch)
    - [Use Json to deserialize Quotes](#use-json-to-deserialize-quotes)
  - [Some Helpers](#some-helpers)
  - [Giants' shoulders](#giants-shoulders)
<!-- /TOC -->

## Some advantages

* Very simple to use and operate, ideal for lots of small projects or personal ones
* Easy html filter with [HObject](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_HObject.cs) (a HtmlNode wrap with use similar to JObject)
* Internal conversion from html to XElement, no need to external tools on use
* Automatic Json parser to JObject
* Automatic Json deserialize <T>
* Modular Parser engine (you can add your own parsers!)
  * JSON and XML already included
* Modular Caching engine (you can add your own!)
  * Stand alone Cache engine included, no need to external softwares
* Modular Downloader engine (you can add your own!)
  * WebClient with cookies or HttpClient download engine included

Easy **import with [NuGet](https://www.nuget.org/packages/RafaelEstevam.Simple.Spider)**

## Installation

Install the SimpleSpider [NuGet package](https://www.nuget.org/packages/RafaelEstevam.Simple.Spider): `Install-Package RafaelEstevam.Simple.Spider`

## Getting started

1. Start a new console project and add Nuget Reference
2. PM> `Install-Package RafaelEstevam.Simple.Spider`
3. Create a class for your spider (or leave in program)
4. create a new instance of SimpleSpider
   1. Give it a name, cache and log will be saved with that name
   2. Give it a domain (your spider will not fleet from it)
5. Add a event `FetchCompleted` to 
6. Optionally give a first page with `AddPage`. If omitted, it will use the home page of the domain
7. Call `Execute()`

``` C#
void run()
{
    var spider = new SimpleSpider("QuotesToScrape", new Uri("http://quotes.toscrape.com/"));
    // Set the completed event to implement your stuff
    spider.FetchCompleted += fetchCompleted_items;
    // execute
    spider.Execute();
}
void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
{
    // walk around ...
    // TIP: inspect args to see stuff

    var hObj = args.GetHObject();
    string[] quotes = hObj["span > .text"];
}
```

> TIP: Use the [Simple.Tests](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Test/Sample) project to see examples and poke around

## Samples

Inside the [Simple.Tests](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Test/Sample) folders are all samples, these are some of them:

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
    // HObject also processes XPath
    var hObj = args.GetHObject();
    // collect book data
    var articleProd = hObj.XPathSelect("//article[@class=\"product_page\"]");
    if (articleProd.IsEmpty()) return; // not a book
    // Book info
    string sTitle = articleProd.XPathSelect("//h1");
    string sPrice = articleProd.XPathSelect("//p[@class=\"price_color\"]");
    string sStock = articleProd.XPathSelect("//p[@class=\"instock availability\"]").GetValue().Trim();
    string sDesc = articleProd.XPathSelect("p")?.GetValue(); // books can be description less
}
```

Below we have the same example but using HObject to select html elements
```C#
void run() ... /* Same run() method */
void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
{
    // ignore all pages except the catalogue
    if (!args.Link.ToString().Contains("/catalogue/")) return;

    var hObj = args.GetHObject();
    // collect book data
    var articleProd = hObj["article > .product_page"]; // XPath: "//article[@class=\"product_page\"]"
    if (articleProd.IsEmpty()) return; // not a book
    // Book info
    string sTitle = articleProd["h1"];                 // XPath: "//h1"
    string sPrice = articleProd["p > .price_color"];   // XPath: "//p[@class=\"price_color\"]"
    string sStock = articleProd["p > .instock"].GetValue().Trim();// XPath "//p[@class=\"instock\"]"
    string sDesc =  articleProd.Children("p");         // XPath "p"
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/BooksToScrape.cs)*

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
    // Supports css selector style, dot for Class
    HObject ex2B = hObj["span"][".text"];
    // Also supports css '>' selector style
    HObject ex2C = hObj["span > .text"];
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
    string ex14A = hObj["nav"]["ul"]["li"]["a"].GetAttributeValue("href"); // Specify one attribute
    string ex14B = hObj["nav"]["ul"]["li"]["a"].GetHrefValue(); // directly
    // Multiple parameters can be parametrized as array
    string ex14C = hObj["nav", "ul", "li", "a"].GetHrefValue();
    // Multiple parameters can filtered with ' > '
    string ex14D = hObj["nav > ul > li > a"].GetHrefValue();
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/QuotesToScrape_HObject.cs)*


### Easy storage with StorageEngines

Store you data with Attached Storage Engines, some included !

```C#
void run()
{
    var iP = new InitializationParams()
                // Defines a Storage Engine
                // All stored items will be in spider folder as JsonLines
                .SetStorage(new Storage.JsonLinesStorage()); 
    var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"), iP);
    // callback to gather items
    spider.FetchCompleted += fetchCompleted_items;
    // execute
    spider.Execute();
}
static void fetchCompleted_items(object Sender, FetchCompleteEventArgs args)
{
    // ignore all pages except the catalogue
    if (!args.Link.ToString().Contains("/catalogue/")) return;

    var tag = new Tag(args.GetDocument());
    var books = tag.SelectTags<Article>("//article[@class=\"product_page\"]");

    foreach (var book in books)
    {
        // process prices
        var priceP = book.SelectTag<Paragraph>(".//p[@class=\"price_color\"]");
        var price = priceP.InnerText.Trim();
        // Store name and prices
        (Sender as SimpleSpider).Storage.AddItem(args.Link, new
        {
            name = book.SelectTag("//h1").InnerText,
            price
        });
    }
}
```
*[see full source](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Test/Sample/BooksToScrape_StorageJsonLines.cs)*

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


## Some [Helpers](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Lib/Helper)
* [FetchHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/FetchHelper.cs): Fast single resource fetch with lots of parsers
* [RequestHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/RequestHelper.cs): Make requests (gets and posts) easily
* [XmlSerializerHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/XmlSerializerHelper.cs): Generic class to serialize and deserialize stuff using Xml, easy way to save what you collected without any database
* [CSV Helper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/CSVHelper.cs): Read csv files (even compressed) without external libraries
* [UriHelper](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/UriHelper.cs): Manipulates parts of the Uri
* XElement to Stuff: Extract [tables](https://github.com/RafaelEstevamReis/SimpleSpider/blob/master/Simple.Lib/Helper/XElementHelper.cs#L17) from page in DataTable

## Giants' shoulders
* Html parsing with [Html Agility Pack](https://html-agility-pack.net/)
* Json parsing with [Newtonsoft](https://www.newtonsoft.com/json)
* Logging with [Serilog](https://serilog.net/)
