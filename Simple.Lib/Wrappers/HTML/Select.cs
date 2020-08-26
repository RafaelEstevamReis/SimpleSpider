using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Select : Tag
    {
        public Select(HtmlDocument doc) : base(doc) { }
        public Select(HtmlNode node) : base(node) { }

        public Option[] GetItems()
        {
            return GetChildren<Option>().ToArray();
        }
    }
}
