using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class IFrame : Tag
    {
        public IFrame(HtmlDocument doc) : base(doc) { }
        public IFrame(HtmlNode node) : base(node) { }

        public string Name => Attributes["name"];
        public string Src => Attributes["src"];

        public int Height
        {
            get
            {
                var h = Attributes["height"];
                if (h == null) return 150; // w3 default
                return int.Parse(h);
            }
        }
        public int Width
        {
            get
            {
                var h = Attributes["width"];
                if (h == null) return 300; // w3 default
                return int.Parse(h);
            }
        }
    }
}
