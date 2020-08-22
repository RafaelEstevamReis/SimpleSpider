using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Li : Tag
    {
        public Li(HtmlDocument doc) : base(doc) { }
        public Li(HtmlNode node) : base(node) { }
    }
}
