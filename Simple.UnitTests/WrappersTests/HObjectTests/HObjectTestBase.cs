using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HObjectTests
{
    public class HObjectTestBase
    {
        public static HObject GetHObject()
        {
            return GetHObject(TestHelper.BaseHtml());
        }
        public static HObject GetHObject(string Html)
        {
            return new HObject(HtmlParseHelper.ParseHtmlDocument(Html));
        }
    }
}
