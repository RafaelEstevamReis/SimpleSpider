using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_FilterTests : HObjectTestBase
    {
        [Fact]
        public void Wrappers_HObject_OfID()
        {
            var h = GetHObject();
            Assert.Equal("nTxt2", h["input"].OfID("iTxt2").GetNameValue());
        }

        [Fact]
        public void Wrappers_HObject_OfClass()
        {
            var h = GetHObject();
            Assert.Equal(new string[] { " A = 2 ", " A = 3 " }, h["li"].OfClass("green"));
        }

        [Fact]
        public void Wrappers_HObject_OfWhich()
        {
            var h = GetHObject();
            Assert.Equal("/", h["form"].OfWhich("method", "POST").GetAttributeValue("action"));
        }
    }
}
