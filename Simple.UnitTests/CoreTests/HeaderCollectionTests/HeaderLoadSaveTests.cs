using System;
using System.Linq;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.HeaderCollectionTests
{
    public class HeaderLoadSaveTests
    {
        [Fact]
        public void Core_HeaderTests_Load()
        {
            string[] lines =
            {
                "Key: Value",
                "Key2: Value:A:B", // colons are ok
                "Key3: ", // Empty value is OK
                "" // Empty is ok
            };
            var hdr = HeaderCollection.LoadHeader(lines);
            Assert.Equal(3, hdr.Count);
            Assert.Equal("Value", hdr["Key"]);
            Assert.Equal("Value:A:B", hdr["Key2"]);
            Assert.Equal("", hdr["Key3"]);
        }

        [Fact]
        public void Core_HeaderTests_FormatErrors_EmptyKey()
        {
            string[] lines =
            {
                ": Value",
            };
            Assert.Throws<FormatException>(() => HeaderCollection.LoadHeader(lines));
        }

        [Fact]
        public void Core_HeaderTests_Cycle()
        {
            string[] lines =
            {
                "Content-Type: text/html; charset=UTF-8",
                "Referer: http://quotes.toscrape.com/",
                "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", // Long is ok
            };
            var hdrLoaded = HeaderCollection.LoadHeader(lines);
            Assert.Equal(3, hdrLoaded.Count);
            Assert.Equal("text/html; charset=UTF-8", hdrLoaded["Content-Type"]);
            Assert.Equal("http://quotes.toscrape.com/", hdrLoaded["Referer"]);
            Assert.Equal("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                         hdrLoaded["Accept"]);

            var savedLines = HeaderCollection.SaveHeader(hdrLoaded);

            Assert.Equal(lines, savedLines.ToArray());
        }
    }
}
