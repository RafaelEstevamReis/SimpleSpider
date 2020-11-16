
namespace RafaelEstevam.Simple.Spider.Extensions
{
    /// <summary>
    /// Request Headers extensions
    /// </summary>
    public static class RequestHeaderExtension
    {
        /// <summary>
        /// Adds generic User-Agent, Accept-Language, Accept-Encoding, Accept, Dnt, and Upgrade-Insecure-Requests
        /// </summary>
        /// <param name="Header">Headers reference to add to</param>
        public static void AddBaseRequestHeaders(this HeaderCollection Header)
        {
            Header.AddItem("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
            //Header.AddItem("Accept-Encoding", "gzip, deflate");
            // Refer: -
            Header.AddItem("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*; q = 0.8");
            Header.AddItem("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Safari/537.36");
            Header.AddItem("Dnt", "1");
            Header.AddItem("Upgrade-Insecure-Requests", "1");
        }
        /// <summary>
        /// Adds AddBaseRequestHeaders() then a firefox UA
        /// </summary>
        /// <param name="Header">Headers reference to add to</param>
        public static void AddFirefoxRequestHeaders(this HeaderCollection Header)
        {
            AddBaseRequestHeaders(Header);
            Header.AddItem("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
        }
        /// <summary>
        /// Adds AddBaseRequestHeaders() then a Chrome Windows10 UA
        /// </summary>
        /// <param name="Header">Headers reference to add to</param>
        public static void AddChromeW10RequestHeaders(this HeaderCollection Header)
        {
            AddBaseRequestHeaders(Header);
            Header.AddItem("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.103 Safari/537.36");
        }
        /// <summary>
        /// Adds AddBaseRequestHeaders() then a Chrome Linux X11 x64 UA
        /// </summary>
        /// <param name="Header">Headers reference to add to</param>
        public static void AddChromeLinuxRequestHeaders(this HeaderCollection Header)
        {
            AddBaseRequestHeaders(Header);
            Header.AddItem("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.103 Safari/537.36");
        }
    }
}
