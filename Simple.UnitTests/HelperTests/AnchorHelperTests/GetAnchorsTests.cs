using System;
using System.Linq;
using RafaelEstevam.Simple.Spider.Helper;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.HelperTests.AnchorHelperTests
{
    public class GetAnchorsTests
    {
        [Fact]
        public void AnchorHelper_GetAnchorsTests_Base()
        {
            var uri = new Uri("http://foo.bar");

            string html = "<a href=\"TEST\"> test </a>";
            var arr = AnchorHelper.GetAnchors(uri, html).ToArray();

            Assert.Equal(new Uri[] { new Uri("http://foo.bar/TEST") }, arr);
        }
        [Fact]
        public void AnchorHelper_GetAnchorsTests_MoreComplex()
        {
            var uri = new Uri("http://foo.bar");

            string html = "<html><div><a></a><li><a href=\"TEST\"> test </a><a href=\"T2\"> test2 </a>";
            var arr = AnchorHelper.GetAnchors(uri, html).ToArray();

            Assert.Equal(new Uri[]
                         {
                             new Uri("http://foo.bar/TEST"),
                             new Uri("http://foo.bar/T2")
                         },
                         arr);
        }
        [Fact]
        public void AnchorHelper_GetAnchorsTests_BrokenHtml_AnchorHRef()
        {
            var uri = new Uri("http://foo.bar");

            string html = "<html><div><a></a><li><a ";
            var arr = AnchorHelper.GetAnchors(uri, html).ToArray();

            Assert.Empty(arr);
        }
        [Fact]
        public void AnchorHelper_GetAnchorsTests_BrokenHtmlAnchor()
        {
            var uri = new Uri("http://foo.bar");

            string html = "<html><div><a></a><li><a href=\"TEST\" id= \"A\"";
            var arr = AnchorHelper.GetAnchors(uri, html).ToArray();

            Assert.Empty(arr);
        }
        [Fact]
        public void AnchorHelper_GetAnchorsTests_Empty()
        {
            var uri = new Uri("http://foo.bar");

            string html = "<a id=\"TEST\"> test </a>";
            var arr = AnchorHelper.GetAnchors(uri, html).ToArray();

            Assert.Empty(arr);
        }
    }
}
