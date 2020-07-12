using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;
using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Net.RafaelEstevam.Spider
{
    public class SimpleSpider
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public Configuration Configuration { get;  }
        public string SpiderName { get; }
        public Uri BaseUri { get; }
        public ICacher Cacher { get; }
        public IDownloader Downloader { get; }

        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;

        public SimpleSpider(string spiderName, Uri baseUri, InitializationParams @params = null)
        {
            this.SpiderName = spiderName;
            this.BaseUri = baseUri;

            this.Cacher = @params?.cacher;
            this.Downloader = @params?.downloader;

            this.Configuration = new Configuration();
            initializeConfiguration(spiderName, @params);
            
            initializeQueues();
            // initialize read-only
            if (Cacher == null) Cacher = new ContentCacher();
            if (Downloader == null) Downloader = new WebClientDownloader();

            initializeFetchers();
        }

        private void initializeConfiguration(string spiderName, InitializationParams init)
        {
            var dir = init?.SpiderDirectory;
            if (dir == null) dir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory;

            var spiderPath = new DirectoryInfo(Path.Combine(dir.FullName, spiderName));
            if (!spiderPath.Exists) spiderPath.Create();
            Configuration.SpiderDirectory = spiderPath;

        }

        private void initializeQueues()
        {
            qAdded = new ConcurrentQueue<Link>();
            qCache = new ConcurrentQueue<Link>();
            qDownload = new ConcurrentQueue<Link>();
            hExecuted = new HashSet<string>();
        }

        private void initializeFetchers()
        {
            Cacher.Initialize(qCache, Configuration);
            Cacher.FetchCompleted += Cacher_FetchCompleted;
            Cacher.FetchFailed += Cacher_FetchFailed;
            Cacher.ShouldFetch += Cacher_ShouldFetch;
            
            Downloader.Initialize(qDownload, Configuration);
            Downloader.FetchCompleted += Downloader_FetchCompleted;
            Downloader.FetchFailed += Downloader_FetchFailed;
            Downloader.ShouldFetch += Downloader_ShouldFetch;
        }

        public void Execute()
        {
            Cacher.Start();
            Downloader.Start();

            int idleTimeout = 0;
            while (true)
            {
                if (workQueue()) continue;

                Thread.Sleep(100);
                if (QueueFinished())
                {
                    if (idleTimeout++ > 10)
                    {
                        break;
                    }
                }
                else
                {
                    idleTimeout = 0;
                }
            }

            Cacher.Stop();
            Downloader.Stop();
        }

        private bool workQueue()
        {
            if (qAdded.TryDequeue(out Link lnk))
            {
                if (alreadyExecuted(lnk.Uri)) return true;

                if (Cacher.HasCache(lnk.Uri))
                {
                    qCache.Enqueue(lnk);
                }
                else
                {
                    qDownload.Enqueue(lnk);
                }
                return true;
            }
            return false;
        }

        public void AddPage(IEnumerable<Uri> PageToVisit, Uri SourcePage)
        {
            foreach (var p in PageToVisit)
            {
                AddPage(p, SourcePage);
            }
        }
        public Link AddPage(Uri PageToVisit, Uri SourcePage)
        {
            return addPage(PageToVisit, SourcePage);
        }
        private Link addPage(Uri pageToVisit, Uri sourcePage)
        {
            if (pageToVisit.Host != BaseUri.Host) return null;            
            if (alreadyExecuted(pageToVisit)) return null;

            var lnk = new Link(pageToVisit, sourcePage);

            var args = new ShouldFetchEventArgs(lnk);
            ShouldFetch?.Invoke(this, args);
            if (args.Cancel) return null;

            qAdded.Enqueue(lnk);
            return lnk;
        }
        private bool alreadyExecuted(Uri pageToVisit)
        {
            return hExecuted.Contains(pageToVisit.ToString());
        }

        #region Scheduler

        private void Downloader_FetchFailed(object Sender, FetchFailEventArgs args)
        {
            hExecuted.Add(args.Link.Uri.ToString());
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

        private void Cacher_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            args.Source = FetchEventArgs.EventSource.Cacher;
            shouldFetch(Sender, args);
        }

        private void Downloader_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            args.Source = FetchEventArgs.EventSource.Downloader;
            shouldFetch(Sender, args);
        }
        private void shouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            if (alreadyExecuted(args.Link))
            {
                args.Cancel = true;
                args.Reason = ShouldFetchEventArgs.Reasons.AlreadyFetched;
                return;
            }
            // Ask user
            ShouldFetch?.Invoke(this, args);
            if (args.Cancel) Console.WriteLine($"[USER CANCEL] {args.Link}");
        }
        #endregion

        private void fetchCompleted(FetchCompleteEventArgs args)
        {
            hExecuted.Add(args.Link.Uri.ToString());
            FetchCompleted?.Invoke(this, args);
        }

        public bool QueueFinished()
        {
            if (Cacher.IsProcessing) return false;
            if (Downloader.IsProcessing) return false;

            if (qAdded.TryPeek(out _)) return false;
            if (qCache.TryPeek(out _)) return false;
            if (qDownload.TryPeek(out _)) return false;

            // Count to be sure
            return QueueSize() == 0;
        }
        public int QueueSize()
        {
            return qAdded.Count
                   + qCache.Count
                   + qDownload.Count;
        }

        public class InitializationParams
        {
            public ICacher cacher { get; set; }
            public IDownloader downloader { get; set; }
            public DirectoryInfo SpiderDirectory { get; set; }
        }
    }
}
