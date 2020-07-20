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
            string href;
            int idx;
            foreach (var a in lstA)
            {
                try
                {
                    idx = a.IndexOf("href");
                    if (idx < 0) continue;
                    href = a.Substring(idx);
                    href = href.Substring(href.IndexOf('"') + 1);
                    href = href.Substring(0, href.IndexOf('"'));

                    if (href.StartsWith("javascript:")) continue;
                }
                catch { continue; }
                yield return new Uri(request, href);
            }
        }
    }
}
