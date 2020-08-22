# SimpleSpider

A simple and modular web spider written in C# .Net Core

 ![.NET Core](https://github.com/RafaelEstevamReis/SimpleSpider/workflows/.NET%20Core/badge.svg)
 [![NuGet](https://buildstats.info/nuget/Net.RafaelEstevam.Spider.Simple.Lib)](http://nuget.org/packages/Net.RafaelEstevam.Spider.Simple.Lib)

# Content
- [SimpleSpider](#simplespider)
- [Content](#content)
- [Some advantages](#some-advantages)
- [Getting started](#getting-started)
- [Samples](#samples)

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

Easy **import with [NuGet](https://www.nuget.org/packages/Net.RafaelEstevam.Spider.Simple.Lib)**

## Getting started

1. Start a new console project and add Nuget Reference
2. PM> Install-Package Net.RafaelEstevam.Spider.Simple.Lib
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

See all samples at [Simple.Tests](https://github.com/RafaelEstevamReis/SimpleSpider/tree/master/Simple.Test/Sample)