using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_LinqHelpers : HObjectTestBase
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
    }
}
