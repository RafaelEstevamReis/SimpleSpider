using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    public class WebClientDownloader : IDownloader
    {
        private ConcurrentQueue<Link> queue;
        private Configuration config;

        Thread thread;
        CustomWebClient webClient;

        public List<TimeSpan> FetchTempo { get;  }

        public WebClientDownloader()
        {
            webClient = new CustomWebClient();
            webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
            FetchTempo = new List<TimeSpan>();
        }

        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

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
            //thread.Interrupt();   
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

                if (queue.TryDequeue(out current))
                {
                    var args = new ShouldFetchEventArgs(current);
                    ShouldFetch(this, args);
                    if (args.Cancel) continue;

                    downloading = true; 
                    config.Logger.Information($"[WEB] {current.Uri}");
                    webClient.EnableCookies = config.Cookies_Enable;
                    current.FetchStart = DateTime.Now;
                    webClient.DownloadDataAsync(current.Uri);
                }
            }
        }
        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            current.FetchEnd = DateTime.Now;
            FetchTempo.Add(current.FetchEnd - current.FetchStart);
            if (FetchTempo.Count > 1000)
            {
                while (FetchTempo.Count > 980)
                    FetchTempo.RemoveRange(0, 25);
            }
            if (e.Error == null)
            {
                var responseHeaders = webClient.ResponseHeaders.AllKeys.Select(k => KeyValuePair.Create(k, webClient.ResponseHeaders.Get(k))).ToArray();
                FetchCompleted(this, new FetchCompleteEventArgs(current, e.Result, webClient.LastRequestHeaders, responseHeaders));
            }
            else
            {
                FetchFailed(this, new FetchFailEventArgs(current, e.Error, webClient.LastRequestHeaders));
            }
            downloading = false;
        }

        class CustomWebClient : WebClient
        {
            public bool EnableCookies { get; set; }
            public CookieContainer CookieContainer { get; }
            public KeyValuePair<string, string>[] LastRequestHeaders { get; private set; }

            public CustomWebClient()
            {
                CookieContainer = new CookieContainer();
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                if (EnableCookies) request.CookieContainer = CookieContainer;
                //LastRequestHeaders = request.Headers.AllKeys.Select(k => KeyValuePair.Create(k, request.Headers.Get(k))).ToArray();

                return request;
            }
        }
    }
}
