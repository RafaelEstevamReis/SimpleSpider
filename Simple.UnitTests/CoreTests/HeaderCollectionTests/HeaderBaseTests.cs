using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.CoreTests.HeaderCollectionTests
{
    public class HeaderBaseTests
    {
        [Fact]
        public void Core_HeaderTests_HttpCtor()
        {
            string[] lines =
            {
                "Content-Type: text/html; charset=UTF-8",
                "Referer: http://quotes.toscrape.com/",
            };
            string[] keys =
            {
                "Content-Type",
                "Referer",
            };
            var hdr = new HeaderCollection(lines);
            testHeader(hdr);
            Assert.Equal(keys, hdr.AllKeys);

            hdr.AddItem("Accept", "text/html,application/xhtml+xml");
            Assert.Equal(3, hdr.Count);
            Assert.Equal("text/html,application/xhtml+xml", hdr["Accept"]);
        }

        [Fact]
        public void Core_HeaderTests_NVCCtor()
        {
            var nvc = new NameValueCollection();
            nvc["Content-Type"] = "text/html; charset=UTF-8";
            nvc["Referer"] = "http://quotes.toscrape.com/";

            var hdr = new HeaderCollection(nvc);
            testHeader(hdr);
        }
        [Fact]
        public void Core_HeaderTests_NVCAdd()
        {
            var nvc = new NameValueCollection();
            nvc["Content-Type"] = "text/html; charset=UTF-8";
            nvc["Referer"] = "http://quotes.toscrape.com/";

            var hdr = new HeaderCollection();
            hdr.AddItems(nvc);
            testHeader(hdr);
        }

        [Fact]
        public void Core_HeaderTests_KVPCtor()
        {
            var kvp = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8"),
                new KeyValuePair<string, string>("Referer", "http://quotes.toscrape.com/"),
            };

            var hdr = new HeaderCollection(kvp);
            testHeader(hdr);

            hdr = new HeaderCollection();
            hdr.AddItems(kvp);
            testHeader(hdr);
        }
        [Fact]
        public void Core_HeaderTests_KVPAdd()
        {
            var kvp = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8"),
                new KeyValuePair<string, string>("Referer", "http://quotes.toscrape.com/"),
            };

            var hdr = new HeaderCollection();
            hdr.AddItems(kvp);
            testHeader(hdr);
        }

        internal static void testHeader(HeaderCollection hdr)
        {
            Assert.Equal(2, hdr.Count);
            Assert.Equal("text/html; charset=UTF-8", hdr["Content-Type"]);
            Assert.Equal("http://quotes.toscrape.com/", hdr["Referer"]);
        }
    }
}
