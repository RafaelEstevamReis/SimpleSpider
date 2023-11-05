using Newtonsoft.Json;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Fetchs Pages
    /// </summary>
    public static class PageFetcherHelepr
    {
        private static HttpClient client;

        static void initialize()
        {
            if (client != null) return;

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            };
            client = new HttpClient(handler);
        }
        /// <summary>
        /// Requests a Page
        /// </summary>
        public static async Task<PageResponse> RequestAsync(HttpRequestMessage request)
        {
            initialize();

            var response = await client.SendAsync(request);
            return await PageResponse.BuildAsync(response);
        }
        /// <summary>
        /// Gets a Page
        /// </summary>
        public static async Task<PageResponse> GetHtmlAsync(string url)
        {
            initialize();

            var response = await client.GetAsync(url);
            return await PageResponse.BuildAsync(response);
        }
    }
    /// <summary>
    /// Fetchs Pages
    /// </summary>
    public class PageFetcher
    {
        private HttpClient client;

        public PageFetcher() { }
        public PageFetcher(HttpClientHandler hdl)
        {
            client = new HttpClient(hdl);
        }
        public PageFetcher(HttpClient client) { this.client = client; }

        void checkInitialize()
        {
            if (client != null) return;

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            };
            client = new HttpClient(handler);
        }
        /// <summary>
        /// Requests a Page
        /// </summary>
        public async Task<PageResponse> RequestAsync(HttpRequestMessage request)
        {
            checkInitialize();

            var response = await client.SendAsync(request);
            return await PageResponse.BuildAsync(response);
        }
        /// <summary>
        /// Gets a Page
        /// </summary>
        public async Task<PageResponse> GetHtmlAsync(string url)
        {
            checkInitialize();

            var response = await client.GetAsync(url);
            return await PageResponse.BuildAsync(response);
        }
    }
    public class PageResponse
    {
        private HttpResponseMessage OriginalResponse;

        private string responseString;
        private Encoding encoding = Encoding.UTF8;
        public Encoding Encoding
        {
            get => encoding; set
            {
                encoding = value;
                responseString = null;
            }
        }

        /// <summary>
        /// Gets the collection of HTTP response headers
        /// </summary>
        public HttpResponseHeaders Headers { get; protected set; }
        /// <summary>
        /// Gets the collection of HTTP content headers
        /// </summary>
        public HttpContentHeaders ContentHeaders { get; protected set; }
        /// <summary>
        /// Gets the request message which led to this response message
        /// </summary>
        public HttpRequestMessage RequestMessage { get; protected set; }
        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful
        /// </summary>
        public bool IsSuccessStatusCode { get; protected set; }
        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with
        /// the status code
        /// </summary>
        public string ReasonPhrase { get; protected set; }
        /// <summary>
        /// Gets the status code of the HTTP response
        /// </summary>
        public HttpStatusCode StatusCode { get; protected set; }
        /// <summary>
        /// Gets the string response of the request
        /// </summary>
        public string ResponseString { get => responseString ??= Encoding.GetString(Response); }
        /// <summary>
        /// Gets the original bytes
        /// </summary>
        public byte[] Response { get; private set; }
        /// <summary>
        /// Gets a HObject from response 
        /// </summary>
        public HObject AsHObject() => HObject.FromHTML(ResponseString);
        /// <summary>
        /// Deserialize a json
        /// </summary>
        public T FromJson<T>() => JsonConvert.DeserializeObject<T>(ResponseString);
        /// <summary>
        /// Deserialize a xml
        /// </summary>
        public T FromXml<T>() where T : new()
        {
            using var ms = new MemoryStream(Response);
            using var reader = new StreamReader(ms, Encoding);
            return XmlSerializerHelper.Deserialize<T>(reader);
        }

        public void EnsureSuccessStatusCode()
            => OriginalResponse.EnsureSuccessStatusCode();

        internal static async Task<PageResponse> BuildAsync(HttpResponseMessage response)
        {
            return new PageResponse()
            {
                OriginalResponse = response,
                Headers = response.Headers,
                ContentHeaders = response.Content.Headers,
                RequestMessage = response.RequestMessage,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Response = await response.Content.ReadAsByteArrayAsync(),
            };
        }
    }
}
