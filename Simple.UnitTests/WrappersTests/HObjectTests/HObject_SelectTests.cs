using Net.RafaelEstevam.Spider.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_SelectTests
    {
        [Fact]
        public void Wrappers_HObject_IDs()
        {
            var h = GetHObject();
            Assert.Equal("nTxt2", h.IDs("iTxt2").GetAttributeValue("name"));
        }

        [Fact]
        public void Wrappers_HObject_Classes()
        {
            var h = GetHObject();
            Assert.Equal(2, h.Classes("green").Count());
        }

        [Fact]
        public void Wrappers_HObject_Having()
        {
            var h = GetHObject();
            Assert.Equal("iTxt2", h.Having("name", "nTxt2").GetIdValue());
        }

        public static HObject GetHObject()
        {
            return HObject_IndexingTests.GetHObject(TestHelper.BaseHtml());
        }
    }
}
