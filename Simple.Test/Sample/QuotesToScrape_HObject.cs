using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
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
            HObject ex2A = hObj["span"].OfClass("text");
            // Supports css selector style, dot for Class
            HObject ex2B = hObj["span"][".text"];
            // Also supports css '>' selector style
            HObject ex2C = hObj["span > .text"];

            // Example 3
            // Get all elements with some arbitrary attribute
            //  Original HTML: <span class="text" itemprop="text">
            HObject ex3 = hObj.Having("itemprop", "text");

            // Example 4
            // Get all Spans filters by some arbitrary attribute
            //  Original HTML: <span class="text" itemprop="text">
            HObject ex4 = hObj["span"].OfWhich("itemprop", "text");

            // Example 5
            // Explicit Tag filter, get all spans with Class='text'
            HObject ex5A = hObj.Tags("span").OfClass("text");

            // Example 6
            //Filter using only indexing, uses example 2 as base
            HObject ex6_ex2 = hObj["span"][HObject.SearchType.FilterClass, "text"];
            // Supports css selector style
            HObject ex_2B = hObj["span"][".text"];

            // Example 7
            // Quotes.toscrape do not have Ids, but ...
            // ... Get all elements with Id='something' (like example 1)
            HObject ex7 = hObj.IDs("something");

            // Example 8
            // Quotes.toscrape do not have Ids, but ...
            // ... Get all spans with Id='something' (like example 2)
            HObject ex8A = hObj["span"].OfID("something");
            // Supports css selector style
            HObject ex8B = hObj["span"]["#something"];

            //Example 9
            // Exports Values as Strings both with GetValues method or implicitly
            string[] ex9A = hObj["span"][".text"].GetValues();
            string[] ex9B = hObj["span"][".text"];

            //Example 10
            // Exports the first Value as String both with GetValue method or implicitly
            string ex10A = hObj["span"][".text"].GetValue();
            string ex10B = hObj["span"][".text"];
            // N-th item can be easily get by
            string ex10C = hObj["span > .text"][2]; // Get the 3rd item, index[2]

            //Example 11
            // Exports elements as XElements both with GetXElements method or implicitly
            IEnumerable<XElement> ex11A = hObj["span"][".text"].GetXElements();
            XElement[] ex11B = hObj["span"][".text"];

            //Example 12
            // Exports first element as XElement both with GetXElement method or implicitly
            XElement ex12A = hObj["span"][".text"].GetXElement();
            XElement ex12B = hObj["span"][".text"];

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
    }
}
