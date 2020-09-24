using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    /// <summary>
    /// Simple Downloader using System.Net.Http.HttpClient
    /// </summary>
    public class HttpClientDownloader : IDownloader
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
        /// Occurs before fetch to check if it should fetch this resource
        /// </summary>
        public event ShouldFetch ShouldFetch;
        /// <summary>
        /// Occurs before fetch to allow request manipulation
        /// </summary>
        public event FetchT<HttpRequestMessage> BeforeRequest;
        /// <summary>
        /// Collection of Headers to be included on the Request
        /// </summary>
        public HeaderCollection IncludeRequestHeaders { get; }

        private ConcurrentQueue<Link> queue;
        private Configuration config;
        CancellationTokenSource cancellationToken;

        Thread thread;
        HttpClient httpClient;

        /// <summary>
        /// Creates a HttpClientDownloader instance
        /// </summary>
        /// <param name="AddDefaultHeaders">Specify when initialize some default headers</param>
        public HttpClientDownloader(bool AddDefaultHeaders = false)
        {
            var hdl = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            httpClient = new HttpClient(hdl);
            cancellationToken = new CancellationTokenSource();
            IncludeRequestHeaders = new HeaderCollection();
            if (AddDefaultHeaders)
            {
                Extensions.RequestHeaderExtension.AddBaseRequestHeaders(IncludeRequestHeaders);
                IncludeRequestHeaders["Accept-Encoding"] = "gzip, deflate";
            }
        }
        /// <summary>
        /// Initialize the downloader
        /// </summary>
        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            queue = WorkQueue;
            config = Config;
            thread = new Thread(doStuff);
        }
        /// <summary>
        /// Indicates when is processing a resource
        /// </summary>
        public bool IsProcessing { get; private set; }
        /// <summary>
        /// Starts the Downloader operation
        /// </summary>
        public void Start()
        {
            run = true;
            thread.Start();
        }
        /// <summary>
        /// Stops the Downloader operation
        /// </summary>
        public void Stop()
        {
            cancellationToken.Cancel();
            run = false;
        }

        bool run;
        private void doStuff(object obj)
        {
            IsProcessing = false;
            while (run)
            {
                Thread.Sleep(Math.Max(100, config.DownloadDelay));
                if (cancellationToken.Token.IsCancellationRequested) break;

                if (IsProcessing) continue;

                if (config.Paused || config.Paused_Downloader)
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (queue.TryDequeue(out Link current))
                {
                    var args = new ShouldFetchEventArgs(current);
                    ShouldFetch(this, args);
                    if (args.Cancel) continue;

                    IsProcessing = true;
                    config.Logger.Information($"[WEB] {current.Uri.UrlWithoutHost()}");
                    current.FetchStart = DateTime.Now;

                    try
                    {
                        fetch(current);
                    }
                    catch (Exception ex)
                    {
                        FetchFailed(this, new FetchFailEventArgs(current, 0, ex, new HeaderCollection()));
                    }

                    IsProcessing = false;
                }
            }
        }
        private void fetch(Link current)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, current.Uri);
            RequestHelper.mergeHeaders(req, IncludeRequestHeaders);

            BeforeRequest?.Invoke(this, new FetchTEventArgs<HttpRequestMessage>(current, req));

            var resp = httpClient.SendAsync(req).Result;
            var reqHeaders = RequestHelper.processHeaders(req.Headers);

            current.FetchEnd = DateTime.Now;
            if (resp.IsSuccessStatusCode)
            {
                var respHeaders = RequestHelper.processHeaders(resp.Headers);

                byte[] content = RequestHelper.loadResponseDataDecompress(resp.Content.ReadAsByteArrayAsync().Result);

                FetchCompleted(this, new FetchCompleteEventArgs(current,
                                  content,
                                  reqHeaders,
                                  respHeaders));
            }
            else
            {
                if (resp.Headers.Location != null)
                {
                    // Redirect
                    var newUri = current.Uri.Combine(resp.Headers.Location.ToString());
                    current.ResourceMoved(newUri);
                    config.Logger.Information($"[MOV] {current.MovedUri} -> {current.Uri}");
                    fetch(current);
                    return;
                }

                FetchFailed(this, new FetchFailEventArgs(current,
                                                         (int)resp.StatusCode,
                                                         new HttpRequestException($"[{(int)resp.StatusCode}] {resp.ReasonPhrase}"),
                                                         new HeaderCollection(reqHeaders)));
            }
        }
        /// <summary>
        /// Creates a new instance of HttpClientDownloader with Extensions.RequestHeaderExtension.AddBaseRequestHeaders
        /// </summary>
        [Obsolete]
        public static HttpClientDownloader BuildDownloaderWithDefaulGenerictHeadders()
        {
            var http = new HttpClientDownloader();
            Extensions.RequestHeaderExtension.AddBaseRequestHeaders(http.IncludeRequestHeaders);
            return http;
        }
    }
}
