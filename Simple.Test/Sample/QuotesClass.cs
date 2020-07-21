using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Test.Sample
{
    public class QuotesObject
    {
        public bool has_next { get; set; }
        public int page { get; set; }

        public Quote[] quotes { get; set; }
    }
    public class Quote
    {
        public Author author { get; set; }
        public string[] tags { get; set; }
        public string text { get; set; }
    }
    public class Author
    {
        public string goodreads_link { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
    }
}
