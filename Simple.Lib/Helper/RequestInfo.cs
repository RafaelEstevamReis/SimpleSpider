using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Helper class to get information with request
    /// </summary>
    public static class RequestInfo
    {
        static readonly HttpClient httpClient;
        static RequestInfo()
        {
            httpClient = new HttpClient();
        }
        /// <summary>
        /// Send a Get request to an Uri
        /// </summary>
        /// <returns>Information about the request</returns>
        public static Result SendRequest(Uri uri) => SendRequest(uri, HttpMethod.Get, null);
        /// <summary>
        /// Send a request to an Uri
        /// </summary>
        /// <returns>Information about the request</returns>
        public static Result SendRequest(Uri uri, HttpMethod method, HttpContent content)
        {
            var req = new HttpRequestMessage(method, uri);

            var dtInicio = DateTime.UtcNow; // UTCNow is faster

            if (content != null) req.Content = content;
            var resp = httpClient.SendAsync(req).Result;

            var duration = DateTime.UtcNow - dtInicio;

            Result result = new Result();
            result.RequestUri = uri;
            result.RequestMethod = method;
            result.RequestHeaders = RequestHelper.processHeaders(req.Headers);
            result.StatusCode = resp.StatusCode;
            result.ReasonPhrase = resp.ReasonPhrase;
            result.ResponseHeaders = RequestHelper.processHeaders(resp.Headers);
            result.RawContent = RequestHelper.loadResponseDataDecompress(resp.Content.ReadAsByteArrayAsync().Result);
            result.Content = Encoding.UTF8.GetString(result.RawContent);
            result.RequestDuration = duration;

            return result;
        }
        /// <summary>
        /// Request result information
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Uri used
            /// </summary>
            public Uri RequestUri { get; set; }
            /// <summary>
            /// Method used
            /// </summary>
            public HttpMethod RequestMethod { get; set; }
            /// <summary>
            /// Status code received
            /// </summary>
            public HttpStatusCode StatusCode { get; set; }
            /// <summary>
            /// Status reason phrase
            /// </summary>
            public string ReasonPhrase { get; set; }
            /// <summary>
            /// Headers used on request
            /// </summary>
            public HeaderCollection RequestHeaders { get; set; }
            /// <summary>
            /// Headers received on response
            /// </summary>
            public HeaderCollection ResponseHeaders { get; set; }
            /// <summary>
            /// Content received
            /// </summary>
            public byte[] RawContent { get; set; }
            /// <summary>
            /// Content received as UTF8
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// Request duration time
            /// </summary>
            public TimeSpan RequestDuration { get; set; }
        }
    }
}
