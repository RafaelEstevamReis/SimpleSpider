using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObject_GetAttributesTests : HObjectTestBase
    {
        [Fact]
        public void Wrappers_HObject_GetValue()
        {
            var h = GetHObject();
            Assert.Equal("Option 2", h["li > #opt2"].GetValue());
        }
        [Fact]
        public void Wrappers_HObject_GetValues()
        {
            var h = GetHObject();
            Assert.Equal(new string[]
                         {
                            "Option 1",
                            "Option 2",
                            "Option 3"
                         },
                         h["ol > li"].GetValues());
        }

        [Fact]
        public void Wrappers_HObject_GetAttributeValue()
        {
            var h = GetHObject();
            Assert.Equal("/", h["form"].GetAttributeValue("action"));
        }
        [Fact]
        public void Wrappers_HObject_GetAttributeValues()
        {
            var h = GetHObject();
            Assert.Equal(new string[] { "UTF-8" }, h["meta"].GetAttributeValues("charset"));
        }

        [Fact]
        public void Wrappers_HObject_GetHrefValue()
        {
            var h = GetHObject();
            Assert.Equal("?a=1", h["li"].OfClass("blue")["a"].GetHrefValue());
        }
        [Fact]
        public void Wrappers_HObject_GetHrefValues()
        {
            var h = GetHObject();
            Assert.Equal(new string[] { "?a=2", "?a=3" }, h["li"].OfClass("green")["a"].GetHrefValues());
        }

        [Fact]
        public void Wrappers_HObject_GetClassValue()
        {
            var h = GetHObject();
            Assert.Equal("cTest2", h["div > #iTest2"].GetClassValue());
        }

        [Fact]
        public void Wrappers_HObject_GetIdValue()
        {
            var h = GetHObject();
            Assert.Equal("iTest2", h["div > .cTest2"].GetIdValue());
        }
        [Fact]
        public void Wrappers_HObject_GetIdsValues()
        {
            var h = GetHObject();
            Assert.Equal(new string[] { "iTest1", "iTest2", "iTest3" },
                         h["div"].GetIdsValues());
        }

        [Fact]
        public void Wrappers_HObject_GetNameValue()
        {
            var h = GetHObject();
            Assert.Equal("nTxt3", h["input > #iTxt3"].GetNameValue());
        }
        [Fact]
        public void Wrappers_HObject_GetNameValues()
        {
            var h = GetHObject();
            Assert.Equal(new string[] { "nTxt1", "nTxt2", "nTxt3", "nTxt4", null },
                         h["input"].GetNameValues());
        }

        [Fact]
        public void Wrappers_HObject_GetStyleValue()
        {
            var h = GetHObject();
            Assert.Equal("color:blue;", h["p"][1].GetStyleValue());
        }

    }
}
