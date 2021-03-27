using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Extensions
{
    /// <summary>
    /// Helper for Uri stuff
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Combines a Uri with a Relative Url into a new combined Uri
        /// </summary>
        /// <param name="parent">Base Uri</param>
        /// <param name="relative">Relative Url</param>
        /// <param name="RemoveWhitespace">Indicates whenever it should remove all whitespaces from Url before combining</param>
        /// <returns>New combined Uri</returns>
        public static Uri Combine(this Uri parent, string relative, bool RemoveWhitespace = false)
        {
            if (RemoveWhitespace) relative = RemoveWhitespaceChars(relative);

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
        /// <summary>
        /// Returns Url without the host part
        /// </summary>
        /// <param name="uri">Uri to return from</param>
        /// <returns>Returns the Url</returns>
        public static string UrlWithoutHost(this Uri uri)
        {
            return uri.PathAndQuery;
        }

        /// <summary>
        /// Returns Uri.Query items as a NameValueCollection
        /// </summary>
        /// <param name="uri">Uri containing the query string to parse</param>
        /// <returns>A System.Collections.Specialized.NameValueCollection of query parameters and values</returns>
        public static NameValueCollection GetQueryInfo(this Uri uri)
        {
            return System.Web.HttpUtility.ParseQueryString(uri.Query);
        }

        /// <summary>
        /// Remove whitespace from string <a href="https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794"/>
        /// </summary>
        public static string RemoveWhitespaceChars(this string str)
        {
            // split then join, but is fast enough without using UNSAFE
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        /// <summary>
        /// Splits a Uri in its components: Host + Segments without final slashes
        /// </summary>
        /// <param name="uri">Uri to be splitted</param>
        /// <returns>Parts splitted</returns>
        public static IEnumerable<string> SplitParts(this Uri uri)
        {
            yield return uri.Host;

            foreach (var s in uri.Segments.Skip(1))
            {
                if (s.EndsWith("/"))
                {
                    yield return s[..^1];
                }
                else
                {
                    yield return s;
                }
            }
        }
    }
}
