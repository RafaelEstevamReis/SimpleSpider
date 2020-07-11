using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Net.RafaelEstevam.Spider
{
    public class SimpleSpider
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;

        public Configuration Configuration { get;  }
        public string SpiderName { get; }
        public Uri BaseUri { get; }
        public ICacher Cacher { get; }
        public IDownloader Downloader { get; }


        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;

        public SimpleSpider(string spiderName, Uri baseUri, ICacher cacher, IDownloader downloader)
        {
            this.SpiderName = spiderName;
            this.BaseUri = baseUri;
            this.Cacher = cacher;
            this.Downloader = downloader;

            this.Configuration = new Configuration();

            qAdded = new ConcurrentQueue<Link>();
            qCache = new ConcurrentQueue<Link>();
            qDownload = new ConcurrentQueue<Link>();
            hExecuted = new HashSet<string>();

            cacher.Initialize(qCache, Configuration);
            cacher.FetchCompleted += Cacher_FetchCompleted;
            cacher.FetchFailed += Cacher_FetchFailed;
            downloader.Initialize(qDownload, Configuration);
            downloader.FetchCompleted += Downloader_FetchCompleted;
            downloader.FetchFailed += Downloader_FetchFailed;
        }

        public SimpleSpider(string spiderName, Uri baseUri)
            : this(spiderName, baseUri, null, null)
        {
        }

        public void Execute()
        {
            while (true)
            {
                Thread.Sleep(10);
                workQueue();
            }
        }

        private void workQueue()
        {
            if (qAdded.TryDequeue(out Link lnk))
            {
                if (alreadyExecuted(lnk.Uri)) return;

                if (Cacher.HasCache(lnk.Uri))
                {
                    qCache.Enqueue(lnk);
                }
                else
                {
                    qDownload.Enqueue(lnk);
                }
            }
        }

        public void AddPage(IEnumerable<Uri> PageToVisit, Uri SourcePage)
        {
            foreach (var p in PageToVisit) AddPage(p, SourcePage);
        }
        public void AddPage(Uri PageToVisit, Uri SourcePage)
        {
            addPage(PageToVisit, SourcePage);
        }
        private void addPage(Uri pageToVisit, Uri sourcePage)
        {
            if (alreadyExecuted(pageToVisit)) return;

            var lnk = new Link(pageToVisit, sourcePage);
            qAdded.Enqueue(lnk);
        }
        private bool alreadyExecuted(Uri pageToVisit)
        {
            return hExecuted.Contains(pageToVisit.ToString());
        }

        #region Scheduler

        private void Downloader_FetchFailed(object Sender, FetchFailEventArgs args)
        {
            // TODO Log error
            args.Source = FetchEventArgs.EventSource.Downloader;
            FetchFailed?.Invoke(this, args);
        }

        private void Downloader_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            Cacher.GenerateCacheFor(args);
            args.Source = FetchEventArgs.EventSource.Downloader;
            fetchCompleted(args);
        }

        private void Cacher_FetchFailed(object Sender, FetchFailEventArgs args)
        {
            qDownload.Enqueue(args.Link);
        }

        private void Cacher_FetchCompleted(object Sender, FetchCompleteEventArgs args)
        {
            args.Source = FetchEventArgs.EventSource.Cacher;
            fetchCompleted(args);
        }


        #endregion

        private void fetchCompleted(FetchCompleteEventArgs args)
        {
            FetchCompleted?.Invoke(this, args);
        }


    }
}
