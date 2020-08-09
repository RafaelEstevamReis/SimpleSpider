using System;
using System.Collections.Generic;
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
            var lnk = new Link(LinkUseTests.Books, LinkUseTests.Quotes);
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

            Assert.Equal(376, arr.Length);
            // nothing has changed
            string hex = generateHexMd5(arr);
            Assert.Equal("2942BDC33D7138273B320C9E260B9AF3", hex);
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
