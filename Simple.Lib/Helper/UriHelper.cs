using Serilog.Sinks.File;
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

        public static bool HasFragment(this Uri uri)
        {
            return !string.IsNullOrEmpty(uri.Fragment);
        }
        public static bool HasQuery(this Uri uri)
        {
            return !string.IsNullOrEmpty(uri.Query);
        }

        public static Uri RemoveFragment(this Uri uri)
        {
            if (!HasFragment(uri)) return uri;
            return new Uri(uri.ToString().Replace(uri.Fragment, ""));
        }
        public static Uri RemoveQuery(this Uri uri)
        {
            if (!HasQuery(uri)) return uri;
            return new Uri(uri.ToString().Replace(uri.Query, ""));
        }
        public static Uri RemoveQueryAndFragment(this Uri uri)
        {
            if (!uri.HasFragment() && !uri.HasQuery()) return uri;

            return uri.RemoveQuery().RemoveFragment();

        }
    }
}
