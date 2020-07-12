using Net.RafaelEstevam.Spider;
using System;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Sample.BooksToScrape.run();

            Console.WriteLine("-END-");
            Console.ReadKey();
        }
    }
}
