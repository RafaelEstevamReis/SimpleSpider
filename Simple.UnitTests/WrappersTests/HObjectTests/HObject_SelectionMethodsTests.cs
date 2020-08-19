using Net.RafaelEstevam.Spider.Wrappers;
using System.Linq;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_SelectionMethodsTests
    {
        [Fact]
        public void Wrappers_HObject_Tag()
        {
            var h = GetHObject();
            Assert.Equal(5, h.Tags("p").Count());
            Assert.Equal(3, h.Tags("li").Count());
            Assert.False(h.Tags("li").IsEmpty());
            Assert.True(h.Tags("zzz").IsEmpty());
        }

        [Fact]
        public void Wrappers_HObject_Children()
        {
            var h = GetHObject();
            Assert.Equal("Closing arguments", h["body"].Children("p"));
        }

        public static HObject GetHObject()
        {
            return HObject_IndexingTests.GetHObject(TestHelper.BaseHtml());
        }
    }
}
