using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using Net.RafaelEstevam.Spider.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Helper to fetch stuff
    /// </summary>
    public static class FetchHelper
    {
        static WebClient wc;
        static WebClient getClient()
        {
            if (wc == null) wc = new WebClient();
            BeforeFetch?.Invoke(null, wc);
            return wc;
        }

        /// <summary>
        /// Occurs before fetching a resource
        /// </summary>
        public static event EventHandler<WebClient> BeforeFetch;

        /// <summary>
        /// Fetch resource from uri
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <returns>Byte array with data fetched</returns>
        public static byte[] FetchResource(Uri uri)
        {
            return FetchResource(uri, false);
        }
        /// <summary>
        /// Fetch resource from uri with caching supported
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enableCaching">Defines if should use caching</param>
        /// <returns>Byte array with data fetched</returns>
        public static byte[] FetchResource(Uri uri, bool enableCaching)
        {
            string cacheFile = null;
            if (enableCaching)
            {
                cacheFile = generateCacheFileName(uri);
                if (File.Exists(cacheFile))
                {
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} [CACHE] {uri}");
                    return File.ReadAllBytes(cacheFile);
                }
            }

            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} [FETCH] {uri}");
            var data = getClient().DownloadData(uri);

            if (enableCaching) File.WriteAllBytes(cacheFile, data);

            return data;
        }
        private static string generateCacheFileName(Uri uri)
        {
            string uriFileName = $"{uri.Host}{uri.PathAndQuery.Replace("/", ".")}.cache";
            var dir = generateCacheDirName();
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return Path.Combine(dir, uriFileName);
        }
        private static string generateCacheDirName()
        {
            string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            return Path.Combine(dir, "FetchHelper", "CACHE");
        }

        /// <summary>
        /// Fetch resource from uri
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <param name="enableCaching">Defines if should use caching</param>
        /// <returns>String with data fetched</returns>
        public static string FetchResourceText(Uri uri, Encoding enc = null, bool enableCaching = false)
        {
            var data = FetchResource(uri, enableCaching);
            if (enc == null) enc = Encoding.UTF8;

            return enc.GetString(data);
        }

        /// <summary>
        /// Fetch resource from uri and parse a XElement from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <returns>XElement with data fetched</returns>
        public static XElement FetchResourceXElement(Uri uri, Encoding enc = null)
        {
            return HtmlToXElement.Parse(FetchResourceText(uri, enc));
        }
        /// <summary>
        /// Fetch resource from uri and parse a HtmlDocument from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <param name="enableCaching">Defines if should use caching</param>
        /// <returns>HtmlDocument with data fetched</returns>
        public static HtmlDocument FetchResourceDocument(Uri uri, Encoding enc = null, bool enableCaching = false)
        {
            return HtmlParseHelper.ParseHtmlDocument(FetchResourceText(uri, enc, enableCaching));
        }
        /// <summary>
        /// Fetch resource from uri and parse a HObject from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <returns>HObject with data fetched</returns>
        public static HObject FetchResourceHObject(Uri uri, Encoding enc = null)
        {
            return new HObject(FetchResourceDocument(uri, enc));
        }
        /// <summary>
        /// Fetch resource from uri and parse a JObject from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
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
        /// <param name="enc">Defines which encoding should be used</param>
        /// <returns>JObject with data fetched</returns>
        public static JObject FetchResourceJObject(Uri uri, JsonLoadSettings loadSettings, Encoding enc = null)
        {
            return JObject.Parse(FetchResourceText(uri, enc), loadSettings);
        }
        /// <summary>
        /// Fetch resource from uri and deserialize T from it
        /// </summary>
        /// <param name="uri">Uri to fetch from</param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <param name="enableCaching">Defines if should use caching</param>
        /// <returns>T deserialized with data fetched</returns>
        public static T FetchResourceJson<T>(Uri uri, Encoding enc = null, bool enableCaching = false)
        {
            var json = FetchResourceText(uri, enc, enableCaching);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Fetch resource from uri and deserialize T from it
        /// </summary>
        /// <param name="uri">Uri to fetch from></param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <param name="settings">JsonSerializerSettings Settings</param>
        /// <returns>T deserialized with data fetched</returns>
        public static T FetchResourceJson<T>(Uri uri, JsonSerializerSettings settings, Encoding enc = null)
        {
            var json = FetchResourceText(uri, enc);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Fetch resource from uri and deserialize T from it
        /// </summary>
        /// <param name="uri">Uri to fetch from></param>
        /// <param name="enc">Defines which encoding should be used</param>
        /// <param name="enableCaching">Defines if should use caching</param>
        /// <returns>T deserialized with data fetched</returns>
        public static T FetchResourceXml<T>(Uri uri, Encoding enc = null, bool enableCaching = false) where T : new()
        {
            var data = FetchResource(uri, enableCaching);
            using var ms = new MemoryStream(data);
            using var reader = new StreamReader(ms, enc ?? Encoding.UTF8);
            return XmlSerializerHelper.Deserialize<T>(reader);
        }
 }
}
