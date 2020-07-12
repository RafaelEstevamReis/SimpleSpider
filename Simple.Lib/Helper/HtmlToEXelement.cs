using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider.Helper
{
    public class HtmlToEXelement
    {
        public static XElement Parse(string html)
        {
            // Static configs
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
            
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.OptionOutputAsXml = true;
            doc.OptionFixNestedTags = true;

            doc.LoadHtml(html);
            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                using (StringReader reader = new StringReader(writer.ToString()))
                {
                    var x = XElement.Load(reader);
                    // remove Span root
                    if (x.Name.LocalName == "span" && x.FirstNode != null && x.FirstNode is XElement)
                    {
                        XElement fn = (XElement)x.FirstNode;
                        if (fn.Name.LocalName == "html")
                        {
                            return fn;
                        }
                    }
                    return x;
                }
            }
        }
    }
}
