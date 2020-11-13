using RafaelEstevam.Simple.Spider.Wrappers.HTML;
using System.Linq;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class AnchorTests : TagTestsBase
    {
        [Fact]
        public void Wrappers_HtmlAnchor_Base()
        {
            var tUl = GetTag("//ul");
            var lstA = tUl.Children
                          .Select(li => li.GetChildren<Anchor>()
                                          .FirstOrDefault())
                          .ToArray();

            Assert.Equal(4, lstA.Length);
            Assert.Equal("a", lstA[0].TagName);
            Assert.Equal("?a=1", lstA[0].Href);
            Assert.Null(lstA[0].Target);

            Assert.Equal("?a=2", lstA[1].Href);
            Assert.Null(lstA[1].Target);

            Assert.Equal("?a=3", lstA[2].Href);
            Assert.Null(lstA[2].Target);

            Assert.Equal("?a=new", lstA[3].Href);
            Assert.Equal("_blank", lstA[3].Target);
        }
    }
}
