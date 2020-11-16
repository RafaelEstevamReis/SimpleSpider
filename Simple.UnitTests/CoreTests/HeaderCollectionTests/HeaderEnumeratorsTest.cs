using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.HeaderCollectionTests
{
    public class HeaderEnumeratorsTest
    {
        private HeaderCollection newHeader()
        {string[] lines =
            {
                "Referer: http://quotes.toscrape.com/",
                "Content-Type: text/html; charset=UTF-8",
            };
            return new HeaderCollection(lines);
        }

        [Fact]
        public void Core_HeaderTests_Pairs()
        {
            var hdr = newHeader();
            var pairs = hdr.Pairs;

            Assert.Equal(2, pairs.Length);
            Assert.Equal("Referer", pairs[0].Key);
            Assert.Equal("http://quotes.toscrape.com/", pairs[0].Value);
            Assert.Equal("Content-Type", pairs[1].Key);
            Assert.Equal("text/html; charset=UTF-8", pairs[1].Value);

            hdr = new HeaderCollection();
            hdr.Pairs = pairs;
            HeaderBaseTests.testHeader(hdr);
        }
        [Fact]
        public void Core_HeaderTests_Enumerator()
        {
            var hdr = newHeader();
            foreach (var p in hdr)
            {
                if (p.Key == "Referer") Assert.Equal("http://quotes.toscrape.com/", p.Value);
                if (p.Key == "Content-Type") Assert.Equal("text/html; charset=UTF-8", p.Value);
            }
        }
    }
}
