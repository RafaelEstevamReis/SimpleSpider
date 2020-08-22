using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
   public  class Link : Tag
    {
        public Link(HtmlDocument doc) : base(doc) { }
        public Link(HtmlNode node) : base(node) { }

        public string Href => Attributes["href"];
        public string Rel => Attributes["rel"];
    }
}
