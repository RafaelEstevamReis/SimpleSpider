using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HObjectTests
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
