﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var data = FetchResource(uri);
            //var contentType = getClient().ResponseHeaders[HttpResponseHeader.ContentType];
            //if (!string.IsNullOrEmpty(contentType) && enc == null) { }
            if (enc == null) enc = Encoding.UTF8;

            return enc.GetString(data);
        }
        /// <summary>
        /// Fetch resource from uri and parse a XElement from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <returns>XElement with data fetched</returns>
        public static XElement FetchResourceXElement(Uri uri, Encoding enc = null)
        {
            return HtmlToEXelement.Parse(FetchResourceText(uri, enc));
        }
        /// <summary>
        /// Fetch resource from uri and parse a JObject from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <returns>JObject with data fetched</returns>
        public static JObject FetchResourceJObject(Uri uri, Encoding enc = null)
        {
            return JObject.Parse(FetchResourceText(uri, enc));
        }
        /// <summary>
        /// Fetch resource from uri and parse a JObject from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="loadSettings">JsonLoadSettings to parse with</param>
        /// <returns>JObject with data fetched</returns>
        public static JObject FetchResourceJObject(Uri uri, JsonLoadSettings loadSettings, Encoding enc = null)
        {
            return JObject.Parse(FetchResourceText(uri, enc), loadSettings);
        }
        /// <summary>
        /// Fetch resource from uri and deserialize T from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Especify encoding</param>
        /// <returns>T deserialized with data fetched</returns>
        public static T FetchResourceJson<T>(Uri uri, Encoding enc = null)
        {
            var json = FetchResourceText(uri, enc);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// Fetch resource from uri and deserialize T from it
        /// </summary>
        /// <param name="uri">Uri to fetch from></param>
        /// <param name="enc">Especify encoding</param>
        /// <param name="settings">JsonSerializerSettings Settings</param>
        /// <returns>>T deserialized with data fetched</returns>
        public static T FetchResourceJson<T>(Uri uri, JsonSerializerSettings settings, Encoding enc = null)
        {
            var json = FetchResourceText(uri, enc);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
