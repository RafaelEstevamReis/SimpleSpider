using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers.HTML;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class FormTests
    {
        [Fact]
        public void Wrappers_HtmlForm_DeepDescendents()
        {
            string html =
@"<div>
    <form method=""POST"" action=""/"">
        <div class=""left""> 
            <input type=""text"" name=""nTxt1"" value=""vTxt1"">
            <input type=""text"" name=""nTxt2"" value=""vTxt2"">
        </div>
        <div class=""right""> 
            <input type=""text"" name=""nTxt3""   value=""vTxt3"">
            <input type=""hidden"" name=""nTxt4"" value=""vTxt4"">
        </div>
        <select id=""cars"">
            <option value=""car1"">Car 1</option>
            <option value=""car1"">Car 2</option>
        </select>
        <input type=""submit"" id=""btn1"" value=""Insert"">
        <button type=""reset"" class=""button button2"">Reset Form</button>
    </form>
</div>";
            var form = new Tag(HtmlParseHelper.ParseHtmlDocument(html)).SelectTag("//form").Cast<Form>();

            var inputs = form.GetInputs();
            Assert.Equal(5, inputs.Length);

        }
        [Fact]
        public void Wrappers_HtmlForm_DeepDescendentsCheckParent()
        {
            string html =
@"<div>
    <form name=""frmA"" method=""POST"" action=""/"">
        <input type=""text"" name=""nTxt1"" value=""vTxt1"">
        <input type=""text"" name=""nTxt2"" value=""vTxt2"">
    </form>
    <form name=""frmB"" method=""POST"" action=""/"">
        <div class=""right""> 
            <input type=""text"" name=""nTxt3""   value=""vTxt3"">
            <input type=""hidden"" name=""nTxt4"" value=""vTxt4"">
        </div>
        <select id=""cars"">
            <option value=""car1"">Car 1</option>
            <option value=""car1"">Car 2</option>
        </select>
        <input type=""submit"" id=""btn1"" value=""Insert"">
        <button type=""reset"" class=""button button2"">Reset Form</button>
    </form>
</div>";
            var form = new Tag(HtmlParseHelper.ParseHtmlDocument(html)).SelectTag(@"//form[@name=""frmA""]").Cast<Form>();

            var inputs = form.GetInputs();
            Assert.Equal(2, inputs.Length);

        }

    }
}
