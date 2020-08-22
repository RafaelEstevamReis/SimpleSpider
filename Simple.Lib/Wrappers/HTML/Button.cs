using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Button : Tag
    {
        public Button(HtmlDocument doc) : base(doc) { }
        public Button(HtmlNode node) : base(node) { }

        public string Name => Attributes["name"];
        public string Type => Attributes["type"];
        public string Value => Attributes["value"];
        public bool Disabled => Attributes["disabled"] != null;
    }
}
