﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    /// <summary>
    /// Simple Downloader using WebClient
    /// </summary>
    public class WebClientDownloader : IDownloader
    {
        private ConcurrentQueue<Link> queue;
        private Configuration config;
        CancellationTokenSource cancellationToken;

        Thread thread;
        CustomWebClient webClient;

        public CustomWebClient ExposeInternalWebClient()
        {
            return webClient;
        }

        public List<TimeSpan> FetchTempo { get;  }

        public WebClientDownloader()
        {
            cancellationToken = new CancellationTokenSource();

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
            cancellationToken.Cancel();
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
                int delay = config.DownloadDelay;
                if (FetchTempo.Count > 0) delay -= (int)FetchTempo[^1].TotalMilliseconds;
                
                Task.Delay(Math.Max(100, delay), cancellationToken.Token);
                if (cancellationToken.IsCancellationRequested) break;

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
                    webClient.EnableCookies = config.Cookies_Enable;
                    current.FetchStart = DateTime.Now;
                    webClient.DownloadDataAsync(current.Uri);
                }
            }
        }
        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            // We finished
            current.FetchEnd = DateTime.Now;
            // Manage FetchTempo list
            FetchTempo.Add(current.FetchEnd - current.FetchStart);
            if (FetchTempo.Count > 1000)
            {
                while (FetchTempo.Count > 980)
                    FetchTempo.RemoveRange(0, 25);
            }
            // process result
            if (e.Error == null)
            {
                FetchCompleted(this, 
                               new FetchCompleteEventArgs(current, 
                                                          e.Result, 
                                                          new HeaderCollection(webClient.Headers), 
                                                          new HeaderCollection(webClient.ResponseHeaders)));
            }
            else
            {
                Exception ex = e.Error;
                int code = 0;
                if (e.Error is WebException webError)
                {
                    if (webError.Response is HttpWebResponse resp)
                    {
                        code = (int)resp.StatusCode;
                        
                        var loc = resp.Headers[HttpResponseHeader.Location];
                        if (!string.IsNullOrEmpty(loc))
                        {
                            current.ResourceMoved(new Uri(current.Uri, loc));
                            webClient.DownloadDataAsync(current.Uri);

                            config.Logger.Information($"[MOV] {current.MovedUri} -> {current.Uri}");

                            return;
                        }
                    }
                }
                FetchFailed(this, new FetchFailEventArgs(current, code, ex, new HeaderCollection(webClient.Headers)));
            }
            // Can process next
            downloading = false;
        }
        /// <summary>
        /// Internal WebClient overload to expose protected stuff
        /// </summary>
        public class CustomWebClient : WebClient
        {
            /// <summary>
            /// Last request used
            /// </summary>
            public HttpWebRequest LastRequest { get; private set; }
            /// <summary>
            /// Defines if next request should use cookies
            /// </summary>
            public bool EnableCookies { get; set; }
            /// <summary>
            /// Current cookie container
            /// </summary>
            public CookieContainer CookieContainer { get; }

            internal CustomWebClient()
            {
                CookieContainer = new CookieContainer();
            }
            /// <summary>
            /// Return the WebRequest for this WebClient
            /// </summary>
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.AllowAutoRedirect = false; // need to catch new locations
                if (EnableCookies) request.CookieContainer = CookieContainer;

                LastRequest = request;

                return request;
            }
        }
    }
}
