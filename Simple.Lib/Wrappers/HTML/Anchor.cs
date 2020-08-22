using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Anchor : Tag
    {
        public Anchor(HtmlDocument doc) : base(doc) { }
        public Anchor(HtmlNode node) : base(node) { }

        public string Href => Attributes["href"];
        public string Target => Attributes["target"];
    }
}
