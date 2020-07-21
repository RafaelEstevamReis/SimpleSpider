using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    public static class FetchHelper
    {
        static WebClient wc;
        static WebClient getClient()
        {
            if (wc == null) wc = new WebClient();
            return wc;
        }
        /// <summary>
        /// Fetch resource from uri
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <returns>Byte aray with data fetched</returns>
        public static byte[] FetchResource(Uri uri)
        {
            return getClient().DownloadData(uri);
        }
        /// <summary>
        /// Fetch resource from uri
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <returns>String with data fetched</returns>
        public static string FetchResourceText(Uri uri, Encoding enc = null)
        {
            var data = getClient().DownloadData(uri);
            //var contentType = getClient().ResponseHeaders[HttpResponseHeader.ContentType];
            //if (!string.IsNullOrEmpty(contentType) && enc == null) { }
            if (enc == null) enc = Encoding.UTF8;

            return enc.GetString(data);
        }
    }
}
