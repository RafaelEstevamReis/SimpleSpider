
namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    public class QuotesObject
    {
        public bool has_next { get; set; }
        public int page { get; set; }

        public QuoteInfo[] quotes { get; set; }
    }
    public class QuoteInfo
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


    public class Quote
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
    }
}
