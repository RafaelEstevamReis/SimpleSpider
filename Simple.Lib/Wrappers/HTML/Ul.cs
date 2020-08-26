using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Ul : Tag
    {
        public Ul(HtmlDocument doc) : base(doc) { }
        public Ul(HtmlNode node) : base(node) { }

        public Li[] GetItems()
        {
            return GetChildren<Li>().ToArray();
        }
    }
}