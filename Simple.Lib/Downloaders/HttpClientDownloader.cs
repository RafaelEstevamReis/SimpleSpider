using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    public class HttpClientDownloader : IDownloader
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        private ConcurrentQueue<Link> queue;
        private Configuration config;

        Thread thread;
        HttpClient httpClient;

        public HttpClientDownloader()
        {
            httpClient = new HttpClient();
        }

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            this.queue = WorkQueue;
            this.config = Config;
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
                Thread.Sleep(Math.Max(100, config.DownloadDelay));

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
                        FetchFailed(this, new FetchFailEventArgs(current, new HttpRequestException(resp.ReasonPhrase), new HeaderCollection(reqHeaders)));
                    }

                    downloading = false;
                }
            }
        }
    }
}
