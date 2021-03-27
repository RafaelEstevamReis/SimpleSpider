using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Class to request stuff
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// Defines a default looger, if NULL Console.WriteLine(...) will be used
        /// </summary>
        public ILogger Logger { get; set; }


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
        /// <summary>
        /// Gets the internal HttpClient, handle with care
        /// </summary>
        public HttpClient InternalHttpClient => httpClient;

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
            RequestHeaders.AddBaseRequestHeaders();
            RequestHeaders["Accept-Encoding"] = "gzip, deflate";
        }
        /// <summary>
        /// Send an request an an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendGetRequestAsync(Uri uri)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(new Link(uri, uri), req));

            if (Logger != null) Logger.Information($"[GET] {uri}");

            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        /// <summary>
        /// Send a request
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="completeCallback">A callback for completion event</param>
        /// <returns>True if request success, false otherwise</returns>
        public bool SendGetRequest(Uri uri, FetchComplete completeCallback = null)
        {
            return SendRequest(uri, HttpMethod.Get, null, completeCallback);
        }

        /// <summary>
        /// Send a form data as an asynchronous operation
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="FormData">The content the request sends</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendPostRequestAsync(Uri uri, IEnumerable<(string, string)> FormData)
        {
            await SendPostRequestAsync(uri, CreateFormContent(FormData));
        }

        /// <summary>
        /// Send a request as an asynchronous operation
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
            if (Logger != null) Logger.Information($"[POST] {uri}");
            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        /// <summary>
        /// Send a request
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="stringData">The content the request sends</param>
        /// <param name="ContentType">The content type of the request</param>
        public void SendPostRequest(Uri uri, string stringData, string ContentType)
        {
            SendRequest(uri, HttpMethod.Post, new StringContent(stringData, Encoding.UTF8, ContentType), null);
        }
        /// <summary>
        /// Sends a request
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="method">The HTTP method</param>
        /// <param name="content">The contents of HTTP message</param>
        /// <param name="completeCallback">A callback for completion event</param>
        /// <returns>True if request success, false otherwise</returns>
        public bool SendRequest(Uri uri, HttpMethod method, HttpContent content, FetchComplete completeCallback)
        {
            var lnk = new Link(uri, uri);

            var req = new HttpRequestMessage(method, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(lnk, req));

            if (content != null) req.Content = content;

            if (Logger != null) Logger.Information($"[{method.Method}] {uri}");
            var resp = httpClient.SendAsync(req).Result;

            var reqHeaders = processHeaders(req.Headers);
            if (resp.IsSuccessStatusCode)
            {
                var respHeaders = processHeaders(resp.Headers);
                var result = loadResponseDataDecompress(resp.Content.ReadAsByteArrayAsync().Result);

                var args = new FetchCompleteEventArgs(lnk, result, reqHeaders, respHeaders);
                FetchCompleted?.Invoke(this, args);
                completeCallback?.Invoke(this, args);
                return true;
            }
            else
            {
                var fail = new FetchFailEventArgs(lnk,
                                                  (int)resp.StatusCode,
                                                  new HttpRequestException($"[{(int)resp.StatusCode}] {resp.ReasonPhrase}"),
                                                  new HeaderCollection(reqHeaders));
                FetchFailed?.Invoke(this, fail);
                return false;
            }

        }
        /// <summary>
        /// Sends a Post request with a form content
        /// </summary>
        /// <param name="uri">The Uri the request is sent to</param>
        /// <param name="formFields">Form data to be sent</param>
        /// <param name="completeCallback">A callback for completion event</param>
        /// <returns>True if request success, false otherwise</returns>
        public bool SendFormData(Uri uri, NameValueCollection formFields, FetchComplete completeCallback)
        {
            return SendRequest(uri, HttpMethod.Post, CreateFormContent(formFields), completeCallback);
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
        private static HeaderCollection processHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
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

        /// <summary>
        /// Creates a FormContent from data
        /// </summary>
        /// <param name="formData">Form data to be encoded</param>
        /// <returns>A FormUrlEncodedContent object</returns>
        public static FormUrlEncodedContent CreateFormContent(IEnumerable<(string, string)> formData)
        {
            return new FormUrlEncodedContent(formData.Select(p => new KeyValuePair<string, string>(p.Item1, p.Item2)));
        }
        /// <summary>
        /// Creates a FormContent from data
        /// </summary>
        /// <param name="formData">Form data to be encoded</param>
        /// <returns>A FormUrlEncodedContent object</returns>
        public static FormUrlEncodedContent CreateFormContent(NameValueCollection formData)
        {
            return new FormUrlEncodedContent(formData.AllKeys.Select(k => new KeyValuePair<string, string>(k, formData[k])));
        }
    }
}
