using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

        HttpClient httpClient;

        public RequestHelper()
        {
            var hdl = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };
            httpClient = new HttpClient(hdl);

            Extensions.RequestHeaderExtension.AddBaseRequestHeaders(RequestHeaders);
            RequestHeaders["Accept-Encoding"] = "gzip, deflate";
        }

        public async Task SendGetRequestAsync(Uri uri)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(new Link(uri, uri), req));

            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        public void SendGetRequest(Uri uri)
        {
            SendGetRequestAsync(uri).RunSynchronously();
        }

        public async Task SendPostRequestAsync(Uri uri, Stream postData)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri);
            mergeHeaders(req, RequestHeaders);
            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(new Link(uri, uri), req));

            req.Content = new StreamContent(postData);
            var resp = await httpClient.SendAsync(req);

            await processSendResult(req, resp, new Link(uri, uri));
        }
        public void SendPostRequest(Uri uri, Stream postData)
        {
            SendPostRequestAsync(uri, postData).RunSynchronously();
        }

        private async Task processSendResult(HttpRequestMessage req, HttpResponseMessage resp, Link link)
        {
            var reqHeaders = processHeaders(req.Headers);

            if (resp.IsSuccessStatusCode)
            {
                var respHeaders = processHeaders(resp.Headers);
                var content = loadResponseDataDecompress(await resp.Content.ReadAsByteArrayAsync());

                FetchCompleted(this, new FetchCompleteEventArgs(link,
                                  content,
                                  reqHeaders,
                                  respHeaders));
            }
            else
            {
                FetchFailed(this, new FetchFailEventArgs(link,
                                                         (int)resp.StatusCode,
                                                         new HttpRequestException($"[{(int)resp.StatusCode}] {resp.ReasonPhrase}"),
                                                         new HeaderCollection(reqHeaders)));
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
