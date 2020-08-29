using HtmlAgilityPack;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using System;
using System.Linq;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class TagTests : TagTestsBase
    {
        [Fact]
        public void Wrappers_HtmlTag_CtorDocument()
        {
            var doc = GetRootDocument();
            var tag = new Tag(doc);
            // Document selects Html from #document
            Assert.Equal("html", tag.Node.Name);
            Assert.Equal("html", tag.TagName);
        }
        [Fact]
        public void Wrappers_HtmlTag_CtorNode()
        {
            var node = GetRootNode();
            var tag = new Tag(node);
            // Direct Node loading does not selects Html from #document
            Assert.Equal("#document", tag.Node.Name);
            Assert.Equal("#document", tag.TagName);
        }
        [Fact]
        public void Wrappers_HtmlTag_CtorNullDocument()
        {
            // Null document
            Assert.Throws<ArgumentNullException>(() => new Tag((HtmlDocument)null));
        }
        [Fact]
        public void Wrappers_HtmlTag_CtorNullNode()
        {
            // Null node
            Assert.Throws<ArgumentNullException>(() => new Tag((HtmlNode)null));
        }

        [Fact]
        public void Wrappers_HtmlTag_Name()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            Assert.Equal("li", nLi1.Name);
            var tag = new Tag(nLi1);
            Assert.Equal("li", tag.TagName);

            var nInp = root.SelectSingleNode("//input[@id=\"iTxt2\"]");
            Assert.Equal("input", new Tag(nInp).TagName);
        }

        [Fact]
        public void Wrappers_HtmlTag_InnerText()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            var tag = new Tag(nLi1);
            Assert.Equal("Option 1", tag.InnerText);

            var nInp = root.SelectSingleNode("//input[@id=\"iTxt2\"]");
            Assert.Equal("", new Tag(nInp).InnerText);
        }

        [Fact]
        public void Wrappers_HtmlTag_Id()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            Assert.Equal("opt1", nLi1.Id);
            var tag = new Tag(nLi1);
            Assert.Equal("opt1", tag.Id);

            var nInp = root.SelectSingleNode("//input[@id=\"iTxt2\"]");
            Assert.Equal("iTxt2", new Tag(nInp).Id);
        }

        [Fact]
        public void Wrappers_HtmlTag_Class()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            var tag = new Tag(nLi1);
            Assert.Null(tag.Class);

            var nDiv = root.SelectSingleNode("//div[@id=\"iTest3\"]");
            Assert.Equal("cTest3", new Tag(nDiv).Class);
        }

        [Fact]
        public void Wrappers_HtmlTag_Style()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            var tag = new Tag(nLi1);
            Assert.Null(tag.Style);

            var nP = root.SelectSingleNode("//p[2]");
            Assert.Equal("color:blue;", new Tag(nP).Style);
        }

        [Fact]
        public void Wrappers_HtmlTag_Classes()
        {
            var root = GetRootNode();

            var nLi1 = root.SelectSingleNode("//li[@id=\"opt1\"]");
            var tag = new Tag(nLi1);
            Assert.Equal(new string[0], tag.Classes);

            var nDiv = root.SelectSingleNode("//div[@id=\"iTest3\"]");
            Assert.Equal(new string[] { "cTest3" }, new Tag(nDiv).Classes);

            var nNav = root.SelectSingleNode("//nav");
            Assert.Equal(new string[] { "bgwhite", "fgred" }, new Tag(nNav).Classes);
        }

        [Fact]
        public void Wrappers_HtmlTag_TagAttributes()
        {
            var root = GetRootNode();
            var nDiv = root.SelectSingleNode("//div[@id=\"iTest3\"]");
            Assert.Equal(2, new Tag(nDiv).Attributes.Items.Length);
            Assert.Equal(2, new Tag(nDiv).Attributes.Items.Count()); // Enumerate results with Linq
            Assert.Equal("cTest3", new Tag(nDiv).Attributes["class"]);
            Assert.Equal("iTest3", new Tag(nDiv).Attributes["id"]);
            Assert.Null(new Tag(nDiv).Attributes["inexistent"]);
        }

        [Fact]
        public void Wrappers_HtmlTag_SelectTag()
        {
            // All tests are made agains internal Node
            var tag = GetRootTag();
            Assert.Null(tag.SelectTag("//footer"));
            Assert.Equal("nav", tag.SelectTag("//nav").Node.Name);
            Assert.Equal("iTest3", tag.SelectTag("//div[@class=\"cTest3\"]").Node.Id);
            Assert.Equal("Option 1", tag.SelectTag("//li[@id=\"opt1\"]").Node.InnerText);
        }
        [Fact]
        public void Wrappers_HtmlTag_SelectTags()
        {
            // All tests are made agains internal Node
            // Enumerators should not be null
            var tag = GetRootTag();
            Assert.Empty(tag.SelectTags("//footer"));

            Assert.Single(tag.SelectTags("//nav"));
            var allAs = tag.SelectTags("//a");
            Assert.Equal(3, allAs.Count());

            foreach (var a in allAs) Assert.Equal("a", a.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTag_Children()
        {
            var root = GetRootNode();
            var nDiv = root.SelectSingleNode("//div[@id=\"iTest3\"]");
            var tDiv = new Tag(nDiv);
            var tDivChilds = tDiv.Children;

            Assert.Single(tDivChilds);
            Assert.Equal("ol", tDivChilds[0].TagName);

            var tLis = tDivChilds[0].Children;
            Assert.Equal(3, tLis.Length);
            Assert.Equal("opt1", tLis[0].Id);

            var tP = new Tag(root.SelectSingleNode("//p"));
            Assert.Empty(tP.Children);
        }

        [Fact]
        public void Wrappers_HtmlTag_GetChildren()
        {
            var root = GetRootNode();
            var nDiv = root.SelectSingleNode("//div[@id=\"iTest3\"]");
            var tDiv = new Tag(nDiv);
            var tDivChilds = tDiv.GetChildren().ToArray();

            Assert.Single(tDivChilds);
            Assert.Equal("ol", tDivChilds[0].TagName);

            var tLis = tDivChilds[0].GetChildren().ToArray();
            Assert.Equal(3, tLis.Length);
            Assert.Equal("opt1", tLis[0].Id);

            var tP = new Tag(root.SelectSingleNode("//p"));
            Assert.Empty(tP.GetChildren());
        }

        [Fact]
        public void Wrappers_HtmlTag_GetChildrenNamed()
        {
            var root = GetRootNode();
            var tBody = new Tag(root.SelectSingleNode("//body"));
            Assert.Equal(7, tBody.Children.Length);

            var tUnob = tBody.GetChildren("Unobtainium");
            Assert.Empty(tUnob);

            var divs = tBody.GetChildren("div").ToArray();
            Assert.Equal(3, divs.Length);

            Assert.Equal("iTest1", divs[0].Id);
            Assert.Equal("iTest2", divs[1].Id);
            Assert.Equal("iTest3", divs[2].Id);
        }
        /* GetChilds<T> will be on each one's specific test */

    }
}