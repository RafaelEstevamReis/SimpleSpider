using HtmlAgilityPack;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HtmlTests
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
