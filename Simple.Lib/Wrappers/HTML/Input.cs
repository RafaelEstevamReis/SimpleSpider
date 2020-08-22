using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Input : Tag
    {
        public Input(HtmlDocument doc) : base(doc) { }
        public Input(HtmlNode node) : base(node) { }

        public string Name => Attributes["name"];
        public string Src => Attributes["src"];
        public string Type => Attributes["type"];
        public string Value => Attributes["value"];

        public bool Checked => Attributes["checked"] != null;
        public bool Disabled => Attributes["disabled"] != null;
    }
}
