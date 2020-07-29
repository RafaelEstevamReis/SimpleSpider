using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Helper for Uri stuff
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        /// Combines a Uri with a Relative Url into a new combined Uri
        /// </summary>
        /// <param name="parent">Base Uri</param>
        /// <param name="relative">Relative Url</param>
        /// <returns>New combined Uri</returns>
        public static Uri Combine(this Uri parent, string relative)
        {
            // This method allows for future improvement on Uri combining and edge case handling

            return new Uri(parent, relative);
        }
    }
}
