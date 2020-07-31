using System;
using System.Collections.Generic;
using System.Text;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrapers;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    public class QuotesToScrape_HObject
    {
        public static void run()
        {
            // Get Quotes.ToScrape.com as HObject
            HObject hObj = FetchHelper.FetchResourceHObject(new Uri("http://quotes.toscrape.com/"));

            // Example 1
            // Get all elements with Class='text'
            HObject ex1 = hObj.Classes("text");

            // Example 2
            // Get all Spans and filter by Class='text'
            HObject ex2 = hObj["span"].OfClass("text");

            // Example 3
            // Get all elements with some arbitray attribute
            //  Original HTML: <span class="text" itemprop="text">
            HObject ex3 = hObj.Having("itemprop", "text");

            // Example 4
            // Get all Spans filteres by some arbitray attribute
            //  Original HTML: <span class="text" itemprop="text">
            HObject ex4 = hObj["span"].OfWhich("itemprop", "text");

            // Example 5
            // Explicit Tag filter, get all spans with Class='text'
            HObject ex5 = hObj.Tags("span").OfClass("text");

            // Example 6
            //Filter using only indexing, uses example 2 as base
            HObject ex6_ex2 = hObj["span"][HObject.SearchType.FilterClass, "text"];

            // Example 7
            // Quotes.toscrape do not have Ids, but ...
            // ... Get all elements with Id='something' (like example 1)
            HObject ex7 = hObj.IDs("something");

            // Example 8
            // Quotes.toscrape do not have Ids, but ...
            // ... Get all spans with Id='something' (like example 2)
            HObject ex8 = hObj["span"].OfID("something");

        }
    }
}
