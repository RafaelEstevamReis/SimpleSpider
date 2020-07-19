using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;
using Net.RafaelEstevam.Spider.Interfaces;
using Net.RafaelEstevam.Spider.Parsers;

namespace Net.RafaelEstevam.Spider
{
    public class SimpleSpider
    {
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public Configuration Configuration { get; }
        public string SpiderName { get; }
        public Uri BaseUri { get; }
        public ICacher Cacher { get; }
        public IDownloader Downloader { get; }
        public List<IParserBase> Parsers { get; }

        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;

        private List<CollectedData> lstCollected;

        public SimpleSpider(string spiderName, Uri baseUri, InitializationParams @params = null)
        {
            this.SpiderName = spiderName;
            this.BaseUri = baseUri;

            this.Cacher = @params?.cacher;
            this.Downloader = @params?.downloader;

            lstCollected = new List<CollectedData>();
            this.Configuration = @params?.ConfigurationPrototype ?? new Configuration();
            initializeConfiguration(spiderName, @params);

            initializeQueues();
            // initialize read-only
            if (Cacher == null) Cacher = new ContentCacher();
            if (Downloader == null) Downloader = new WebClientDownloader();

            initializeFetchers();
            Parsers = new List<IParserBase>();
        }

        private void initializeConfiguration(string spiderName, InitializationParams init)
        {
            var dir = init?.SpiderDirectory;
            if (dir == null) dir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory;

            var spiderPath = new DirectoryInfo(Path.Combine(dir.FullName, spiderName));
            if (!spiderPath.Exists) spiderPath.Create();
            Configuration.SpiderDirectory = spiderPath;

            var dataPath = new DirectoryInfo(Path.Combine(spiderPath.FullName, "Data"));
            if (!dataPath.Exists) dataPath.Create();
            Configuration.SpiderDataDirectory = dataPath;

            //Configuration.Spider_SaveCollectedFile = Path.Combine(dataPath.FullName, "collected.xml");
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

            if (QueueSize() == 0) addPage(BaseUri, BaseUri);

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

        public void Collect(IEnumerable<object> Object, Uri CollectedOn)
        {
            foreach (var o in Object) Collect(o, CollectedOn);
        }
        public void Collect(object Object, Uri CollectedOn)
        {
            lstCollected.Add(new CollectedData()
            {
                Object = Object,
                CollectedOn = CollectedOn.ToString(),
                CollectAt = DateTime.Now
            });
        }

        public CollectedData[] CollectedItems() { return lstCollected.ToArray(); }

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

            var contentType = args.ResponseHeaders.FirstOrDefault(h => h.Key == "Content-Type");
            if (!string.IsNullOrEmpty(contentType.Value))
            {
                foreach (var p in Parsers)
                {
                    try
                    {
                        if (p.MimeTypes.Any(m => m.Equals(contentType.Value, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            p.Parse(this, args);
                        }
                    }
                    catch { }
                }
            }
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
            public Configuration ConfigurationPrototype { get; set; }
        }
        public class CollectedData
        {
            public object Object { get; set; }
            public string CollectedOn { get; set; }
            public DateTime CollectAt { get; set; }
        }
    }
}
