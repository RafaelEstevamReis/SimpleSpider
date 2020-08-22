using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Div : Tag
    {
        public Div(HtmlDocument doc) : base(doc) { }
        public Div(HtmlNode node) : base(node) { }
    }
}
