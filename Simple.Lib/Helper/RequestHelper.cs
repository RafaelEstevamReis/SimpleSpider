using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Class to request stuff
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
     /// Occurs when fetch is complete 
     /// </summary>
        public event FetchComplete FetchCompleted;
        /// <summary>
        /// Occurs when fetch fails
        /// </summary>
        public event FetchFail FetchFailed;
        /// <summary>
        /// Occurs before fetch to allow request manipulation
        /// </summary>
        public event FetchT<HttpRequestMessage> BeforeRequest;

        /// <summary>
        /// Collection of Headers to be included on the Request
        /// </summary>
        public HeaderCollection RequestHeaders { get; }
        /// <summary>
        /// Cookie container used to store server cookies
        /// </summary>
        public CookieContainer Cookies { get; }

        HttpClient httpClient;
        /// <summary>
        /// Create a new instance
        /// </summary>
        public RequestHelper(bool UseCookies = false)
        {
            var hdl = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };
            if (UseCookies)
            {
                Cookies = new CookieContainer();
                hdl.CookieContainer = Cookies;
                hdl.UseCookies = true;
            }
            else
            {
                hdl.UseCookies = false;
            }
            httpClient = new HttpClient(hdl);
            RequestHeaders = new HeaderCollection();
            Extensions.RequestHeaderExtension.AddBaseRequestHeaders(RequestHeaders);
            RequestHeaders["Accept-Encoding"] = "gzip, deflate";
        }
        /// <summary>
        /// Send an request as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendGetRequestAsync(Uri uri)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(new Link(uri, uri), req));

            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        /// <summary>
        /// Send an request
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        public void SendGetRequest(Uri uri)
        {
            SendGetRequestAsync(uri).Wait();
        }
        /// <summary>
        /// Send an request as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="stringData">The content the request sends</param>
        /// <param name="ContentType">The content type of the request</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendPostRequestAsync(Uri uri, string stringData, string ContentType)
        {
            await SendPostRequestAsync(uri, new StringContent(stringData, Encoding.UTF8, ContentType));
        }
        /// <summary>
        /// Send a form data as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="FormData">The content the request sends</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendPostRequestAsync(Uri uri, IEnumerable<(string,string)> FormData)
        {
            await SendPostRequestAsync(uri, FormData.Select(p => new KeyValuePair<string, string>(p.Item1, p.Item2)));
        }
        /// <summary>
        /// Send a form data as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="FormData">The content the request sends</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendPostRequestAsync(Uri uri, IEnumerable< KeyValuePair<string, string>> FormData)
        {
            var content = new FormUrlEncodedContent(FormData);
            await SendPostRequestAsync(uri, content);
        }

        /// <summary>
        /// Send an request as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="postData">The content the request sends</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendPostRequestAsync(Uri uri, HttpContent postData)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(new Link(uri, uri), req));

            req.Content = postData;
            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        /// <summary>
        /// Send an request
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="stringData">The content the request sends</param>
        /// <param name="ContentType">The content type of the request</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public void SendPostRequest(Uri uri, string stringData, string ContentType)
        {
            SendPostRequestAsync(uri, stringData, ContentType).Wait();
        }

        private async Task processSendResult(HttpRequestMessage req, HttpResponseMessage resp, Link link)
        {
            var reqHeaders = processHeaders(req.Headers);

            if (resp.IsSuccessStatusCode)
            {
                var respHeaders = processHeaders(resp.Headers);
                var content = loadResponseDataDecompress(await resp.Content.ReadAsByteArrayAsync());

                FetchCompleted?.Invoke(this, new FetchCompleteEventArgs(link, content, reqHeaders, respHeaders));
            }
            else
            {
                var fail = new FetchFailEventArgs(link,
                                                  (int)resp.StatusCode,
                                                  new HttpRequestException($"[{(int)resp.StatusCode}] {resp.ReasonPhrase}"),
                                                  new HeaderCollection(reqHeaders));
                FetchFailed?.Invoke(this, fail);
            }
        }

        internal static HeaderCollection processHeaders(HttpResponseHeaders headers)
        {
            return new HeaderCollection(processHeaders(headers.Cast<KeyValuePair<string, IEnumerable<string>>>()));
        }
        internal static HeaderCollection processHeaders(HttpRequestHeaders headers)
        {
            return new HeaderCollection(processHeaders(headers.Cast<KeyValuePair<string, IEnumerable<string>>>()));
        }
        private static HeaderCollection processHeaders(IEnumerable< KeyValuePair<string, IEnumerable<string>>> headers)
        {
            return new HeaderCollection(headers
                    .Select(o => new KeyValuePair<string, string>(o.Key, string.Join(",", o.Value))));
        }
        internal static byte[] loadResponseDataDecompress(byte[] content)
        {
            if (content.Length > 2
                   && content[0] == 0x1f // Gzip magic number
                   && content[1] == 0x8b)
            {
                using var to = new MemoryStream();
                using var from = new MemoryStream(content);
                using var gz = new GZipStream(from, CompressionMode.Decompress);
                gz.CopyTo(to);
                return to.ToArray();
            }
            return content; // pass
        }

        internal static void mergeHeaders(HttpRequestMessage req, HeaderCollection addRequestHeaders)
        {
            if (addRequestHeaders.Count > 0)
            {
                foreach (var pair in addRequestHeaders)
                {
                    if (pair.Value == "")
                    {
                        if (req.Headers.Contains(pair.Key)) req.Headers.Remove(pair.Key);
                    }
                    else
                    {
                        req.Headers.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
    }
}
