using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Ol : Tag
    {
        public Ol(HtmlDocument doc) : base(doc) { }
        public Ol(HtmlNode node) : base(node) { }

        public Li[] GetItems()
        {
            return GetChilds<Li>().ToArray();
        }
    }
}