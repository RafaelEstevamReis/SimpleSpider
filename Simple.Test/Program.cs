using Net.RafaelEstevam.Spider;
using Net.RafaelEstevam.Spider.Test.Sample;
using System;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sample.BooksToScrape.run();
            //Sample.QuotesToScrape_Scroll.run();
            QuotesToScrape_Scroll_Advanced.run();

            Console.WriteLine("-END-");
            Console.ReadKey();
        }
    }
}
