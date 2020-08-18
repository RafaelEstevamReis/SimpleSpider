using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_IndexingTests
    {
        [Fact]
        public void Wrappers_HObject_ElementFilters()
        {
            var h = GetHObject();

            Assert.Equal("My First Heading", h["h1"]);
            Assert.Equal("My First Heading", h[HObject.SearchType.AnyElement, "h1"]);
        }
        [Fact]
        public void Wrappers_HObject_Chaining()
        {
            var h = GetHObject();
            Assert.Equal("My first paragraph.", h["div"]["p"]);
            Assert.Equal("Closing arguments", h["body"][HObject.SearchType.ChildElement, "p"]);
        }
        [Fact]
        public void Wrappers_HObject_CSS_Chaining()
        {
            var h = GetHObject();
            Assert.Equal("My first paragraph.", h["div > p"]);
        }

        [Fact]
        public void Wrappers_HObject_CSS_Selector()
        {
            var h = GetHObject();

            Assert.Equal("My first paragraph.", h["div > .cTest1 > p"]);
            Assert.Equal("My third paragraph.", h["div > .cTest2 > p"]);


            Assert.Equal("My first paragraph.", h["div > #iTest1 > p"]);
            Assert.Equal("My third paragraph.", h["div > #iTest2 > p"]);
        }

        [Fact]
        public void Wrappers_HObject_NumericIndex()
        {
            var h = GetHObject();

            Assert.Equal("My first paragraph.", h["div"][0]["p"]);
            Assert.Equal("My second paragraph.", h["div"][0]["p"][1]);
            Assert.Equal("My third paragraph.", h["div"][1]["p"]);
            Assert.Equal("My fourth paragraph.", h["div"][1]["p"][1]);
        }

        public static HObject GetHObject()
        {
            return GetHObject(TestHelper.BaseHtml());
        }
        public static HObject GetHObject(string Html)
        {
            return new HObject(HtmlToXElement.Parse(Html));
        }
    }
}
