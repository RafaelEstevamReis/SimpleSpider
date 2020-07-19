using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.RafaelEstevam.Spider.Helper
{
    public class AnchorHelper
    {
        /// <summary>
        /// Simple Substring based 'a' tag enumerator
        /// </summary>
        public static IEnumerable<Uri> GetAnchors(Uri request, string htmlContent)
        {
            var lstA = htmlContent.Split("<a ").Skip(1);

            foreach (var a in lstA)
            {
                string href = a.Substring(a.IndexOf("href"));
                href = href.Substring(href.IndexOf('"') + 1);
                href = href.Substring(0, href.IndexOf('"'));
                yield return new Uri(request, href);
            }
        }
    }
}
