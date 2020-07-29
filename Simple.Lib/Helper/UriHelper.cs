using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    public static class UriHelper
    {
        // This class allows for future improvement on Uri

        public static Uri Combine(this Uri parent, string relative)
        {
            return new Uri(parent, relative);
        }
    }
}
