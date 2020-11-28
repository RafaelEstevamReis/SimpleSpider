using System;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class TagCastTests : TagTestsBase
    {
        [Fact]
        public void Wrappers_HtmlTagCast_InvalidTag()
        {
            var tag = GetTag("//meta");
            Assert.Throws<InvalidCastException>(() => tag.Cast<Anchor>());
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Anchor()
        {
            var tag = GetTag("//a");
            var anchor = tag.Cast<Anchor>();
            Assert.Equal("a", anchor.TagName);
            Assert.Equal("?a=1", anchor.Href);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Article()
        {
            var tag = GetTag("//article");
            var btn = tag.Cast<Article>();
            Assert.Equal("article", btn.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Button()
        {
            var tag = GetTag("//button");
            var btn = tag.Cast<Button>();
            Assert.Equal("button", btn.TagName);
            Assert.Equal("reset", btn.Type);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Data()
        {
            var tag = GetTag("//data");
            var btn = tag.Cast<Data>();
            Assert.Equal("data", btn.TagName);
            Assert.Equal("123", btn.Value);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Div()
        {
            var tag = GetTag("//div");
            var element = tag.Cast<Div>();
            Assert.Equal("div", element.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Form()
        {
            var tag = GetTag("//form");
            var element = tag.Cast<Form>();
            Assert.Equal("form", element.TagName);
            Assert.Equal("POST", element.Method);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_IFrame()
        {
            var tag = GetTag("//iframe");
            var element = tag.Cast<IFrame>();
            Assert.Equal("iframe", element.TagName);
            Assert.Equal("https://www.w3schools.com", element.Src);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Img()
        {
            var tag = GetTag("//img");
            var element = tag.Cast<Img>();
            Assert.Equal("img", element.TagName);
            Assert.Equal(500, element.Width);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Input()
        {
            var tag = GetTag("//input");
            var element = tag.Cast<Input>();
            Assert.Equal("input", element.TagName);
            Assert.Equal("text", element.Type);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Ul()
        {
            var tag = GetTag("//ul");
            var element = tag.Cast<Ul>();
            Assert.Equal("ul", element.TagName);
        }
        [Fact]
        public void Wrappers_HtmlTagCast_Ol()
        {
            var tag = GetTag("//ol");
            var element = tag.Cast<Ol>();
            Assert.Equal("ol", element.TagName);
        }
        [Fact]
        public void Wrappers_HtmlTagCast_Li()
        {
            var tag = GetTag("//li");
            var element = tag.Cast<Li>();
            Assert.Equal("li", element.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Label()
        {
            //var tag = GetTag("//label");
            var tag = new Tag(HtmlParseHelper.ParseHtmlDocument(LabelTests.html1));
            var lbl = tag.SelectTag("//label");
            var element = lbl.Cast<Label>();
            Assert.Equal("label", element.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Link()
        {
            var tag = GetTag("//link");
            var element = tag.Cast<Wrappers.HTML.Link>();
            Assert.Equal("link", element.TagName);
            Assert.Equal("stylesheet", element.Rel);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Meta()
        {
            var tag = GetTag("//meta");
            var element = tag.Cast<Meta>();
            Assert.Equal("meta", element.TagName);
            Assert.Equal("UTF-8", element.Charset);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Param()
        {
            var tag = GetTag("//param");
            var element = tag.Cast<Param>();
            Assert.Equal("param", element.TagName);
            Assert.Equal("autoplay", element.Name);
            Assert.Equal("true", element.Value);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Select()
        {
            var tag = GetTag("//select");
            var element = tag.Cast<Select>();
            Assert.Equal("select", element.TagName);
        }
        [Fact]
        public void Wrappers_HtmlTagCast_Option()
        {
            var tag = GetTag("//option");
            var element = tag.Cast<Option>();
            Assert.Equal("option", element.TagName);
            Assert.Equal("car1", element.Value);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Paragraph()
        {
            var tag = GetTag("//p");
            var element = tag.Cast<Paragraph>();
            Assert.Equal("p", element.TagName);
        }

        [Fact]
        public void Wrappers_HtmlTagCast_Span()
        {
            var tag = GetTag("//span");
            var element = tag.Cast<Span>();
            Assert.Equal("span", element.TagName);
        }
    }
}
