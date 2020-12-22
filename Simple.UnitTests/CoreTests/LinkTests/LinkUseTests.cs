using System;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.LinkTests
{
    public class LinkUseTests
    {
        public static Uri Quotes = new Uri("http://quotes.toscrape.com/");
        public static Uri Books = new Uri("http://books.toscrape.com/");
        public static Uri MovedBooks = new Uri("http://books.toscrape.com/Moved");
        public static Uri RewrittenBooks = new Uri("http://books.toscrape.com/RewrittenBooks");

        [Fact]
        public void Core_LinkTests_Create()
        {
            var lnk = new Link(Books, Quotes);
            // Check if Uri is correct
            Assert.Equal(Books, lnk.Uri);
            Assert.Equal(Quotes, lnk.SourceUri);
            // Check if redirections not occured
            Assert.Null(lnk.MovedUri);
            // Check if rewrites not occured
            Assert.Null(lnk.RewrittenUri);
            // Check if callbacks are empty
            Assert.Null(lnk.FetchCompleteCallBack);
            // Check if dates are zero
            Assert.Equal(DateTime.MinValue, lnk.FetchStart);
            Assert.Equal(DateTime.MinValue, lnk.FetchEnd);
        }

        [Theory]
        [InlineData(0, 0, 0)] // Zero leads to zero
        [InlineData(100000, 100000, 0)] // Are equals
        [InlineData(100000, 100001, 1)] // Are equals
        [InlineData(1000000, 1000010, 10)] // Are equals
        [InlineData(1000010, 1000000, -10)] // Are equals
        public void Core_LinkTests_FetchTime(int Start, int End, int Diff)
        {
            DateTime dt = new DateTime(2020, 1, 1);
            var lnk = new Link(Books, Quotes);
            lnk.FetchStart = dt.AddSeconds(Start);
            lnk.FetchEnd = dt.AddSeconds(End);

            int actualDiff = (int)lnk.FetchTime.Seconds;
            Assert.Equal(Diff, actualDiff);
        }

        [Fact]
        public void Core_LinkTests_Move()
        {
            var lnk = new Link(Books, Quotes);
            Assert.Equal(Books, lnk.Uri);

            lnk.ResourceMoved(MovedBooks);
            Assert.Equal(MovedBooks, lnk.Uri);
            Assert.Equal(Books, lnk.MovedUri);
        }

        [Fact]
        public void Core_LinkTests_Rewritten()
        {
            var lnk = new Link(Books, Quotes);
            Assert.Equal(Books, lnk.Uri);

            lnk.ResourceRewritten(RewrittenBooks);
            Assert.Equal(RewrittenBooks, lnk.Uri);
            Assert.Equal(Books, lnk.RewrittenUri);
        }

        [Fact]
        public void Core_LinkTests_Implicit()
        {
            var lnk = new Link(Books, Quotes);
            // String conversion
            Assert.Equal(Books.ToString(), lnk); // Implicit
            Assert.Equal(Books.ToString(), lnk.ToString()); // Override
            // UriConversion
            Assert.Equal(Books, lnk);
        }

        [Fact]
        public void Core_LinkTests_Parts()
        {
            var uri = new Uri("https://www.w3.org/TR/WD-html40-970917/htmlweb.html");
            var lnk = new Link(uri, null);

            var parts = lnk.Parts;

            Assert.Equal(new string[] { "www.w3.org", "TR", "WD-html40-970917", "htmlweb.html" },
                         parts);

        }
    }
}
