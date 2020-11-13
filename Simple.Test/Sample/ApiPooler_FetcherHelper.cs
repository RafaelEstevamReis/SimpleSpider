using System;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class ApiPooler_FetcherHelper
    {
        public static void run()
        {
            var quotes = Helper.FetchHelper.FetchResourceJson<QuotesObject>(new Uri("http://quotes.toscrape.com/api/quotes?page=1"));
            foreach (var quote in quotes.quotes)
            {
                Console.WriteLine($"Quote: {quote.text}");
                Console.WriteLine($"       - {quote.author.name}");
                Console.WriteLine();
            }
        }
    }
}
