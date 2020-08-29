namespace Net.RafaelEstevam.Spider.UnitTests
{
    public static class TestHelper
    {
        public static string BaseHtml()
        {
            return @"<!DOCTYPE html>
<html>
    <head>
        <title>My First HTML</title>
        <meta charset=""UTF-8"">
    </head>  
    <body>
        <nav class=""bgwhite fgred"">
            <ul>
                <li class=""blue"" ><a href=""?a=1""> A = 1 </a></li>
                <li class=""green""><a href=""?a=2""> A = 2 </a></li>
                <li class=""green""><a href=""?a=3""> A = 3 </a></li>
            </ul>
        </nav>
        <h1>My First Heading</h1>
        <div class=""cTest1"" id=""iTest1"">
            <p>My first paragraph.</p>
            <p style=""color:blue;"">My second paragraph.</p>
        </div>
        <div class=""cTest2"" id=""iTest2"">
            <p>My third paragraph.</p>
            <p>My fourth paragraph.</p>
        </div>
        <div class=""cTest3"" id=""iTest3"">
            <ol>
                <li id=""opt1"">Option 1</li>
                <li id=""opt2"">Option 2</li>
                <li id=""opt3"">Option 3</li>  
            </ol>
        </div>
        <p>Closing arguments</p>
        <form method=""POST"" action=""/"">
            <input type=""text"" name=""nTxt1"" id=""iTxt1"">
            <input type=""text"" name=""nTxt2"" id=""iTxt2"">
            <input type=""text"" name=""nTxt3"" id=""iTxt3"">
            <input type=""text"" name=""nTxt4"" id=""iTxt4"">
            <input type=""submit"" id=""btn1"" value=""Insert"">
        </form>
    </body>
</html>";
        }
        public static string AllElementsHtml()
        {
            return @"<!DOCTYPE html>
<html>
    <head>
        <title>My First HTML</title>
        <meta charset=""UTF-8"">
        <link rel=""stylesheet"" href=""styles.css"">
    </head>  
    <body>
        <nav class=""bgwhite fgred"">
            <ul>
                <li class=""blue"" ><a href=""?a=1""> A = 1 </a></li>
                <li class=""green""><a href=""?a=2""> A = 2 </a></li>
                <li class=""green""><a href=""?a=3""> A = 3 </a></li>
                <li class=""new""><a href=""?a=new"" target=""_blank""> New ! </a></li>
            </ul>
        </nav>
        <h1>My First Heading</h1>
        <div class=""cTest1"" id=""iTest1"">
            <p> Shopping list: </p>
            <ol>
                <li id=""opt1"">Option 1</li>
                <li id=""opt2"">Option 2</li>
                <li id=""opt3"">Option 3</li>  
            </ol>
        </div>
        <form method=""POST"" action=""/"">
            <input type=""text"" name=""nTxt1"" id=""iTxt1"">
            <input type=""text"" name=""nTxt2"" id=""iTxt2"">
            <input type=""text"" name=""nTxt3"" id=""iTxt3"">
            <input type=""text"" name=""nTxt4"" id=""iTxt4"">
            <select id=""cars"">
                <option value=""car1"">Car 1</option>
                <option value=""car1"">Car 2</option>
            </select>
            < input type=""submit"" id=""btn1"" value=""Insert"">
            <button type=""reset"" class=""button button2"">Reset Form</button>
        </form>
        <iframe src=""https://www.w3schools.com"" title=""W3Schools Free Online Web Tutorials""></iframe>
        <img src=""img_girl.jpg"" width=""500"" height=""600"">
    </body>
</html>";
        }
    }
}