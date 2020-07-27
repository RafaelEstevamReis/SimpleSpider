using System.Collections.Generic;

namespace Net.RafaelEstevam.Spider
{
    public sealed partial class SimpleSpider
    {
        public sealed class SpiderData
        {
            public SpiderData()
            {
                Error404 = new HashSet<string>();
            }

            public HashSet<string> Error404 { get; set; }

        }
    }
}
