using System;
using System.Linq;
using RafaelEstevam.Simple.Spider.Specialized.Apache;

namespace RafaelEstevam.Simple.Spider.Test.Sample.Specialized
{
    public class ApacheListing
    {
        public static void run()
        {
            var listing = new Listing("http://test.rafaelestevam.net/small/");
            var pages = listing.GetListing(new ListingOptions())
                               .ToArray(); // to better viewing
            var tree = Listing.BuildTree(pages);

            // to see the structure, put a breakpoint [here]
            tree = tree; // put break point here

            foreach (var d in tree.GetAllDescendantsAndSelf())
            {
                Console.WriteLine($"[D] {d.Entity.Uri}");
                foreach (var files in d.Files)
                {
                    Console.WriteLine($" > {files.FileName}");
                }
            }
        }
    }
}
