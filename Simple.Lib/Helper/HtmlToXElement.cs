using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Class to convert Html to XElement, uses HtmlAgilityPack
    /// </summary>
    public class HtmlToXElement
    {
        private static readonly char[] InvalidCharsToRemove = { ':' };
        /// <summary>
        /// Search for invalid Attribute names and remove them
        /// </summary>
        public static bool SearchForInvalidNames { get; set; } = false;

        /// <summary>
        /// Parses an HTML as a XElement
        /// </summary>
        /// <param name="html">Html content to be parsed</param>
        /// <returns>XElement parsed</returns>
        public static XElement Parse(string html)
        {
            // Static configs
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.OptionOutputAsXml = true;
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(html);

            if (SearchForInvalidNames)
            {
                foreach (var n in doc.DocumentNode.Descendants())
                {
                    foreach (var att in n.Attributes.ToArray()) // Creates a copy to remove
                    {
                        if (InvalidCharsToRemove.Any(c => att.Name.Contains(c)))
                        {
                            n.Attributes.Remove(att);
                            continue;
                        }
                    }
                }
            }

            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                string dom = writer.ToString();
                using (StringReader reader = new StringReader(dom))
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
