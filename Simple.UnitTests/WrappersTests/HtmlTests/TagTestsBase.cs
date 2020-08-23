using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class TagTestsBase
    {


        public Tag GetRootTag()
        {
            return new Tag(HtmlParseHelper.ParseHtmlDocument(TestHelper.BaseHtml()));
        }
    }
}
