using HtmlAgilityPack;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers.HTML;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class TagTestsBase
    {

        public HtmlDocument GetRootDocument()
        {
            return HtmlParseHelper.ParseHtmlDocument(TestHelper.BaseHtml());
        }
        public HtmlNode GetRootNode()
        {
            return GetRootDocument().DocumentNode;
        }
        public Tag GetRootTag()
        {
            return new Tag(GetRootNode());
        }

        public Tag GetTag(string XPath)
        {
            return new Tag(HtmlParseHelper.ParseHtmlDocument(TestHelper.AllElementsHtml()))
                       .SelectTag(XPath);
        }
    }
}
