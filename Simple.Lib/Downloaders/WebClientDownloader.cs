using Microsoft.VisualBasic.CompilerServices;
using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    public class WebClientDownloader : IDownloader
    {
        private ConcurrentQueue<Link> queue;
        private Configuration config;

        Thread thread;
        WebClient webClient;


        public WebClientDownloader()
        {
            webClient = new WebClient();
            webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
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
                    Console.WriteLine($"[WEB] {current.Uri}");
                    webClient.DownloadDataAsync(current.Uri);
                }
            }
        }
        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                var responseHeaders = webClient.ResponseHeaders.AllKeys.Select(k => KeyValuePair.Create(k, webClient.ResponseHeaders.Get(k))).ToArray();
                FetchCompleted(this, new FetchCompleteEventArgs(current, e.Result, responseHeaders));
            }
            else
            {
                FetchFailed(this, new FetchFailEventArgs(current, e.Error));
            }
            downloading = false;
        }
    }
}
