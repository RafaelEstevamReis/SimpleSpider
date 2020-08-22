using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Option : Tag
    {
        public Option(HtmlDocument doc) : base(doc) { }
        public Option(HtmlNode node) : base(node) { }

        public string Label => Attributes["label"];
        public string Value => Attributes["value"];

        public bool Selected => Attributes["selected"] != null;
        public bool Disabled => Attributes["disabled"] != null;
    }
}
