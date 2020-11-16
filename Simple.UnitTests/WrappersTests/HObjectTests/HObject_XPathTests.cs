using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_XPathTests : HObjectTestBase
    {
        [Fact]
        public void Wrappers_HObjectXPath_Base()
        {
            var h = GetHObject();
            Assert.Equal("My First Heading", h.XPathSelect("//h1"));
        }

        [Fact]
        public void Wrappers_HObjectXPath_ID()
        {
            var h = GetHObject();
            Assert.Equal("nTxt2", h.XPathSelect("//input[@id=\"iTxt2\"]").GetNameValue());
            Assert.Equal("My first paragraph.", h.XPathSelect("//div[@id=\"iTest1\"]/p"));
        }

        [Fact]
        public void Wrappers_HObjectXPath_Class()
        {
            var h = GetHObject();
            Assert.Equal("My first paragraph.", h.XPathSelect("//div[@class=\"cTest1\"]/p"));
        }

    }
}
