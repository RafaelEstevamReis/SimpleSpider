using HtmlAgilityPack;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Wrappers.HTML
{
    public class Form : Tag
    {
        public Form(HtmlDocument doc) : base(doc) { }
        public Form(HtmlNode node) : base(node) { }

        public string Action => Attributes["action"];
        public string Method => Attributes["method"];
        public string Name => Attributes["name"];
        public string Target => Attributes["target"];

        public Input[] GetInputs()
        {
            return GetChilds<Input>().ToArray();
        }
    }
}
