using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    /// <summary>
    /// Simple Downloader using System.Net.Http.HttpClient
    /// </summary>
    public class HttpClientDownloader : IDownloader
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        private ConcurrentQueue<Link> queue;
        private Configuration config;
        CancellationTokenSource cancellationToken;

        Thread thread;
        HttpClient httpClient;

        public HttpClientDownloader()
        {
            var hdl = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            httpClient = new HttpClient(hdl);
            cancellationToken = new CancellationTokenSource();
        }

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            queue = WorkQueue;
            config = Config;
            thread = new Thread(doStuff);
        }
        public bool IsProcessing => downloading;

        public void Start()
        {
            run = true;
            thread.Start();
        }

        public void Stop()
        {
            cancellationToken.Cancel();
            run = false;
        }


        bool run;
        bool downloading;
        Link current;
        private void doStuff(object obj)
        {
            downloading = false;
            while (run)
            {
                Task.Delay(Math.Max(100, config.DownloadDelay), cancellationToken.Token);
                if (cancellationToken.Token.IsCancellationRequested) break;

                if (downloading) continue;

                if (config.Paused || config.Paused_Downloader)
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (queue.TryDequeue(out current))
                {
                    var args = new ShouldFetchEventArgs(current);
                    ShouldFetch(this, args);
                    if (args.Cancel) continue;

                    downloading = true;
                    config.Logger.Information($"[WEB] {current.Uri}");
                    current.FetchStart = DateTime.Now;

                    fetch(current);

                    downloading = false;
                }
            }
        }

        private void fetch(Link current)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, current.Uri);
            var resp = httpClient.SendAsync(req).Result;

            var reqHeaders = req
                .Headers
                .Cast<KeyValuePair<string, IEnumerable<string>>>()
                .Select(o => new KeyValuePair<string, string>(o.Key, string.Join(",", o.Value)));

            current.FetchEnd = DateTime.Now;
            if (resp.IsSuccessStatusCode)
            {
                var respHeaders = resp
                    .Headers
                    .Cast<KeyValuePair<string, IEnumerable<string>>>()
                    .Select(o => new KeyValuePair<string, string>(o.Key, string.Join(",", o.Value)));

                FetchCompleted(this, new FetchCompleteEventArgs(current,
                                  resp.Content.ReadAsByteArrayAsync().Result,
                                  new HeaderCollection(reqHeaders),
                                  new HeaderCollection(respHeaders)));
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
    }
}
