using System;
using Net.RafaelEstevam.Spider.Test.Sample;

namespace Net.RafaelEstevam.Spider.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Spider Samples:
            //Sample.BooksToScrape.run();
            //Sample.QuotesToScrape_Scroll.run();
            //QuotesToScrape_Scroll_Advanced.run();

            // Helper Samples
            //ApiPooler_FetcherHelper.run();

            // IgnoreMe file to internal screwing around
            //  Add a file named IgnoreMe.cs with a static void run() on it to play arround with the spider
            //IgnoreMe.run();


            SimpleSpider.HowToUse_PrintToConsole();

            Console.WriteLine("-END-");
            Console.ReadKey();
        }
    }
}
