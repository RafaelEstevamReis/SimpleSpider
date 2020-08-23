using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class TagTests : TagTestsBase
    {
        [Fact]
        public void Wrappers_HtmlTag_SelectTag()
        {
            // Test Select first, all tests use this
            // All tests are made agains internal Node
            var tag = GetRootTag();
            Assert.Null(tag.SelectTag("//footer"));
            Assert.Equal("nav", tag.SelectTag("//nav").Node.Name);
            Assert.Equal("iTest3", tag.SelectTag("//div[@class=\"cTest3\"]").Node.Id);
            Assert.Equal("Option 1", tag.SelectTag("//li[@id=\"opt1\"]").Node.InnerText);
        }

        [Fact]
        public void Wrappers_HtmlTag_Tag()
        {
            var tag = GetRootTag();
            Assert.Null(tag.SelectTag("//footer"));
            Assert.Equal("nav", tag.SelectTag("//nav").TagName);


        }
    }
}
