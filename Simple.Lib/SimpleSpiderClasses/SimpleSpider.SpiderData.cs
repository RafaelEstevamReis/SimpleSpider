using System.Collections.Generic;

namespace Net.RafaelEstevam.Spider
{
    public sealed partial class SimpleSpider
    {
        /// <summary>
        /// Internal spider data storage
        /// </summary>
        public sealed class SpiderData
        {
            /// <summary>
            /// Constructor of the class
            /// </summary>
            public SpiderData()
            {
                Error404 = new HashSet<string>();
            }
            /// <summary>
            /// List of pages that once got and 404 error and should not be downloaded again
            /// </summary>
            public HashSet<string> Error404 { get; set; }

        }
    }
}
