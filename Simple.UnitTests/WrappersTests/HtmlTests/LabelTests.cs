using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class LabelTests
    {
        // https://developer.mozilla.org/en-US/docs/Web/HTML/Element/label

        private static readonly string htmlNothing = @"<label>Nothing to say ...</label>";
        private static readonly string htmlEmpty = @"
<label for=""peas"">Do you like peas?
  <a href=""toWhere?"" id=""peas"">
</label>";

        public static readonly string html1 = @"
<div class=""preference"">
    <label for=""cheese"">Do you like cheese?</label>
    <input type=""checkbox"" name=""cheese"" id=""cheese"">
</div>
<div class=""preference"">
    <label for=""peas"">Do you like peas?</label>
    <input type=""checkbox"" name=""peas"" id=""peas"">
</div>";
        private static readonly string html2 = @"
<label>Do you like peas?
  <input type=""checkbox"" name=""peas"">
</label>";
        

        [Fact]
        public void Wrappers_HtmlLabel_Nothing()
        {
            var lbl = build(htmlNothing);

            Assert.Equal("label", lbl.TagName);
            Assert.Null(lbl.For);
            Assert.Null(lbl.ForElement);
            Assert.Equal("Nothing to say ...", lbl.InnerText);
        }
        [Fact]
        public void Wrappers_HtmlLabel_Empty()
        {
            var lbl = build(htmlEmpty);

            Assert.Equal("peas", lbl.For);
            Assert.Null(lbl.ForElement);
        }

        [Fact]
        public void Wrappers_HtmlLabel_Indirect()
        {
            var lbl = build(html2);

            Assert.Null(lbl.For);
            Assert.Equal("input", lbl.ForElement.TagName);

            var inp = lbl.ForElement.Cast<Input>();
            Assert.Equal("checkbox", inp.Type);
            Assert.Equal("peas", inp.Name);
        }


        public static Label build(string Html)
        {
            return new Tag(HtmlParseHelper.ParseHtmlDocument("<html>" + Html + "</html>")).SelectTag<Label>();
        }
    }
}