using System;

namespace RafaelEstevam.Simple.Spider.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // See here some samples
            // Uncomment and use the Debug Step Into feature (F11) to see the action

            // Spider Samples:
            //Sample.BooksToScrape.run();
            //Sample.QuotesToScrape_Chaining.run();
            //Sample.QuotesToScrapeAPI_Chaining.run(); // similar, but Json-based (API)
            //Sample.QuotesToScrapeAPI_Scroll.run();
            //Sample.QuotesToScrapeAPI_Scroll_Deserialize.run();

            // Helper Samples
            //Sample.QuotesToScrape_Login.run();    // Capture a form element and submit it
            //Sample.ApiPooler_FetcherHelper.run(); // Easy fetch of single resources

            // Easy-parsing sample, use the Debug Step Into feature (F11) to see the action
            //Sample.QuotesToScrape_HObject.run(); // Parse quotes with HObject

            //Storage Engines
            //Sample.BooksToScrape_StorageJsonLines.run();
            //ModulesSamples.Storage_Sqlite_Quotes.run();

            // Specialied modules
            Sample.Specialized.ApacheListing.run();

            // IgnoreMe file to internal screwing around
            //  Add a file named IgnoreMe.cs with a static void run() on it to play around with the spider
            //IgnoreMe.run();

            SimpleSpider.HowToUse_PrintToConsole();

            Console.WriteLine("-END-");
            Console.ReadKey();
        }
    }
}
