using System;
using System.IO;
using System.Text;
using Net.RafaelEstevam.Spider.UnitTests.CoreTests.LinkTests;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.Events
{
    public class FetchCompleteSaveLoadTests
    {
        [Fact]
        public void Events_FetchCompleteEventArgs_Save_Base()
        {
            var lnk = new Link(LinkUseTests.Books, LinkUseTests.Quotes)
            {
                FetchStart = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FetchEnd = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            var bytes = Encoding.ASCII.GetBytes("TEST TEST TEST");

            var req = new HeaderCollection();
            req.AddItem("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9");

            var resp = new HeaderCollection();
            resp.AddItem("Content-Type", "text/html; charset=UTF-8");
            resp.AddItem("Content-Length", "14");

            var args = new FetchCompleteEventArgs(lnk, bytes, req, resp);

            using var ms = new MemoryStream();
            FetchCompleteEventArgs.SaveFetchResult(args, ms);
            var arr = ms.ToArray();
            var strComp = Encoding.ASCII.GetString(arr);

            Assert.Equal(
@"--------------------LINK
Uri: http://books.toscrape.com/
SourceUri: http://quotes.toscrape.com/
FetchStart: 2020-01-01T00:00:00+00:00
FetchEnd: 2020-01-01T00:00:00+00:00

--------------------REQ-HDR
Accept: text/html,application/xhtml+xml,application/xml;q=0.9

--------------------RES-HDR
Content-Type: text/html; charset=UTF-8
Content-Length: 14

TEST TEST TEST", strComp);
        }
        private string generateHexMd5(FetchCompleteEventArgs args)
        {
            using var ms = new MemoryStream();
            FetchCompleteEventArgs.SaveFetchResult(args, ms);
            return generateHexMd5(ms.ToArray());
        }
        private string generateHexMd5(byte[] arr)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(arr);
            return BitConverter.ToString(md5).Replace("-", "");
        }

        [Fact]
        public void Events_FetchCompleteEventArgs_Cycle()
        {
            var lnk = new Link(LinkUseTests.Books, LinkUseTests.Quotes);
            var bytes = Encoding.ASCII.GetBytes("TEST TEST TEST");

            var req = new HeaderCollection();
            req.AddItem("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9");

            var resp = new HeaderCollection();
            resp.AddItem("Content-Type", "text/html; charset=UTF-8");
            resp.AddItem("Content-Length", "14");

            var argsTest = new FetchCompleteEventArgs(lnk, bytes, req, resp);
            using var ms = new MemoryStream();
            FetchCompleteEventArgs.SaveFetchResult(argsTest, ms);
            ms.Position = 0;

            var argsLoaded = FetchCompleteEventArgs.LoadFetchResult(ms);

            checkEqual(argsTest, argsLoaded);
        }

        private void checkEqual(FetchCompleteEventArgs argsTest, FetchCompleteEventArgs argsLoaded)
        {
            // Link
            Assert.Equal(argsTest.Link, argsLoaded.Link);
            Assert.Equal(argsTest.RequestHeaders, argsLoaded.RequestHeaders);
            Assert.Equal(argsTest.ResponseHeaders, argsLoaded.ResponseHeaders);
            Assert.Equal(argsTest.Result, argsLoaded.Result);

            string expected = generateHexMd5(argsTest);
            string actual = generateHexMd5(argsLoaded);

            Assert.Equal(expected, actual);
        }
    }
}
