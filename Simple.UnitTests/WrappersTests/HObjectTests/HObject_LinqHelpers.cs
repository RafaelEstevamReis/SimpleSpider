using Net.RafaelEstevam.Spider.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_LinqHelpers
    {
        [Fact]
        public void Wrappers_HObject_IsEmpty()
        {
            var h = GetHObject();
            Assert.False(h.Tags("li").IsEmpty());
            Assert.True(h.Tags("zzz").IsEmpty());
        }

        [Fact]
        public void Wrappers_HObject_HasAttribute()
        {
            var h = GetHObject();
            Assert.False(h.Tags("form").HasAttribute("id"));
            Assert.True(h.Tags("form").HasAttribute("action"));
        }

        public static HObject GetHObject()
        {
            return HObject_IndexingTests.GetHObject(TestHelper.BaseHtml());
        }
    }
}
