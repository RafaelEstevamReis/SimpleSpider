using HtmlAgilityPack;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Img : Tag
    {
        public Img(HtmlDocument doc) : base(doc) { }
        public Img(HtmlNode node) : base(node) { }

        public string Src => Attributes["src"];
        public string Alt => Attributes["alt"];
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
