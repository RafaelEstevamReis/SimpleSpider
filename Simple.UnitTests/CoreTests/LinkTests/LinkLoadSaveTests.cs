using System;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.CoreTests.LinkTests
{
    public class LinkLoadSaveTests
    {
        [Fact]
        public void Core_LinkTests_Load()
        {
            string[] lines = 
            {
                "Uri: http://quotes.toscrape.com/",
                "SourceUri: http://books.toscrape.com/"
            };

            var lnk = Link.LoadLink(lines);

            Assert.Equal(LinkUseTests.Quotes, lnk.Uri);
            Assert.Equal(LinkUseTests.Books, lnk.SourceUri);
        }

        [Fact]
        public void Core_LinkTests_SmallCycle()
        {
            var baseLink = new Link(LinkUseTests.Books, LinkUseTests.Quotes);

            var lines = Link.SaveLink(baseLink);
            var loaded = Link.LoadLink(lines);

            Assert.Equal(LinkUseTests.Books, loaded.Uri);
            Assert.Equal(LinkUseTests.Quotes, loaded.SourceUri);
        }

        [Fact]
        public void Core_LinkTests_FullCycle()
        {
            var start = new DateTime(2020, 1, 1);
            var end = new DateTime(2020, 1, 2);

            var baseLink = new Link(LinkUseTests.Books, LinkUseTests.Quotes)
            {
                FetchStart = start,
                FetchEnd = end,
            };
            // Order matters
            baseLink.ResourceMoved(LinkUseTests.MovedBooks);
            baseLink.ResourceRewritten(LinkUseTests.RewrittenBooks);

            var lines = Link.SaveLink(baseLink);
            var loaded = Link.LoadLink(lines);

            Assert.Equal(LinkUseTests.RewrittenBooks, loaded.Uri); // last was rewritten
            Assert.Equal(LinkUseTests.Quotes, loaded.SourceUri);
            Assert.Equal(LinkUseTests.Books, loaded.MovedUri); // at move, was the original
            Assert.Equal(LinkUseTests.MovedBooks, loaded.RewrittenUri); // was moved before

            Assert.Equal(start, loaded.FetchStart);
            Assert.Equal(end, loaded.FetchEnd);
        }
    }
}
