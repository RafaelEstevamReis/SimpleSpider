using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Meta : Tag
    {
        public Meta(HtmlDocument doc) : base(doc) { }
        public Meta(HtmlNode node) : base(node) { }

        public string Charset => Attributes["charset"];
        public string HttpEquiv => Attributes["http-equiv"];
        public string Name => Attributes["name"];
        public string Content => Attributes["content"];
        public string[] ContentItems
        {
            get
            {
                string cnt = Content;
                if (cnt == null) return new string[0];
                return Content.Split(',');
            }
        }
    }
}
