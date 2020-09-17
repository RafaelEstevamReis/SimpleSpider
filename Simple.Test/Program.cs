using System;

namespace Net.RafaelEstevam.Spider.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Spider Samples:
            //Sample.BooksToScrape.run();
            //Sample.BooksToScrape_StorageJsonLines.run();
            //Sample.QuotesToScrape_Scroll.run();
            //Sample.QuotesToScrape_Scroll_Deserialize.run();

            // Helper Samples, use the Debug Step Into feature (F11) to see the action
            //Sample.QuotesToScrape_Login.run();    // Capture a form element and submit it
            //Sample.ApiPooler_FetcherHelper.run(); // Easy fetch of single resources

            // Easy-parsing sample, use the Debug Step Into feature (F11) to see the action
            //Sample.QuotesToScrape_HObject.run(); // Parse quotes with HObject

            // IgnoreMe file to internal screwing around
            //  Add a file named IgnoreMe.cs with a static void run() on it to play around with the spider
            //IgnoreMe.run();


            SimpleSpider.HowToUse_PrintToConsole();

            Console.WriteLine("-END-");
            Console.ReadKey();
        }
    }
}
