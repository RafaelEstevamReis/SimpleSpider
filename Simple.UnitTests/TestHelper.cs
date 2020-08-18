using System;
using System.Collections.Generic;
using System.Text;

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
        <nav>
            <ul>
                <li class=""blue""><a href = ""?a=1""> A = 1 </a></li>
                <li class=""green""><a href = ""?a=2""> A = 2 </a></li>
                <li class=""green""><a href = ""?a=3""> A = 3 </a></li>
            </ul>
        </nav>
        <h1>My First Heading</h1>
        <div class=""cTest1"" id=""iTest1"">
            <p>My first paragraph.</p>
            <p>My second paragraph.</p>
        </div>
        <div class=""cTest2"" id=""iTest2"">
            <p>My third paragraph.</p>
            <p>My fourth paragraph.</p>
        </div>
        <p>Closing arguments</p>
    </body>
</html>";
        }        
    }
}
