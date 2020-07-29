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

        /// <summary>
        /// Returns if uri has a Fragment
        /// </summary>
        /// <param name="uri">Uri to check</param>
        /// <returns>Returns true if it has a fragment, else otherwise</returns>
        public static bool HasFragment(this Uri uri)
        {
            return !string.IsNullOrEmpty(uri.Fragment);
        }
        /// <summary>
        /// Returns if uri has a Query
        /// </summary>
        /// <param name="uri">Uri to check</param>
        /// <returns>Returns true if it has a query, else otherwise</returns>
        public static bool HasQuery(this Uri uri)
        {
            return !string.IsNullOrEmpty(uri.Query);
        }
        /// <summary>
        /// Removes the fragment from a Uri
        /// </summary>
        /// <param name="uri">Uri to remove from</param>
        /// <returns>A new Uri without the fragment</returns>
        public static Uri RemoveFragment(this Uri uri)
        {
            if (!HasFragment(uri)) return uri;
            return new Uri(uri.ToString().Replace(uri.Fragment, ""));
        }
        /// <summary>
        /// Removes the query from a Uri
        /// </summary>
        /// <param name="uri">Uri to remove from</param>
        /// <returns>A new Uri without the query</returns>
        public static Uri RemoveQuery(this Uri uri)
        {
            if (!HasQuery(uri)) return uri;
            return new Uri(uri.ToString().Replace(uri.Query, ""));
        }
        /// <summary>
        /// Removes the query and/or the fragment from a Uri
        /// </summary>
        /// <param name="uri">Uri to remove from</param>
        /// <returns>A new Uri without the query and fragment</returns>
        public static Uri RemoveQueryAndFragment(this Uri uri)
        {
            if (!uri.HasFragment() && !uri.HasQuery()) return uri;

            return uri.RemoveQuery().RemoveFragment();

        }
    }
}
