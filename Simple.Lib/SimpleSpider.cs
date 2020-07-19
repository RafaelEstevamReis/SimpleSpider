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
using Serilog;
using Serilog.Core;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Simple spider class
    /// </summary>
    public sealed class SimpleSpider
    {
        /// <summary>
        /// Resource fetched completed
        /// </summary>
        public event FetchComplete FetchCompleted;
        /// <summary>
        /// Resource fetched failed
        /// </summary>
        public event FetchFail FetchFailed;
        /// <summary>
        /// Check if some resource should be fetched
        /// Used to block list urls
        /// </summary>
        public event ShouldFetch ShouldFetch;

        /// <summary>
        /// Spider configurations and parameters
        /// </summary>
        public Configuration Configuration { get; }
        /// <summary>
        /// Name of the spider
        /// </summary>
        public string SpiderName { get; }
        /// <summary>
        /// Base Uri to fetch, resources outside this host will not be fetched
        /// </summary>
        public Uri BaseUri { get; }
        /// <summary>
        /// Current Cacher
        /// </summary>
        public ICacher Cacher { get; }
        /// <summary>
        /// Current Downloader
        /// </summary>
        public IDownloader Downloader { get; }
        /// <summary>
        /// Current Parsers
        /// </summary>
        public List<IParserBase> Parsers { get; }

        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;

        private List<CollectedData> lstCollected;
        private Logger log; // short reference, is accessible through configuration

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
            Parsers = new List<IParserBase>() { new HtmlXElementParser(), new XmlXElementParser(), new JsonParser() };
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


            Configuration.Spider_LogFile = Path.Combine(spiderPath.FullName, $"{ spiderName }.log");
            log = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File(Configuration.Spider_LogFile, rollingInterval: RollingInterval.Day)
               .CreateLogger();
            Configuration.Logger = log;
            log.Information("Initialization complete");
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
        /// <summary>
        /// Main execution loop, returns once finished
        /// </summary>
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

        /// <summary>
        /// Add page to fetch
        /// </summary>
        /// <param name="PagesToVisit">Uris to fetch</param>
        /// <param name="SourcePage">Uri where all the PagesToVisit was found</param>
        public void AddPage(IEnumerable<Uri> PagesToVisit, Uri SourcePage)
        {
            foreach (var p in PagesToVisit)
            {
                AddPage(p, SourcePage);
            }
        }
        /// <summary>
        /// Add page to fetch
        /// </summary>
        /// <param name="PageToVisit">Uri to fetch</param>
        /// <param name="SourcePage">Uri where the PageToVisit was found</param>
        /// <returns>Link object</returns>
        public Link AddPage(Uri PageToVisit, Uri SourcePage)
        {
            return addPage(PageToVisit, SourcePage);
        }
        private Link addPage(Uri pageToVisit, Uri sourcePage)
        {
            if (pageToVisit.Host != BaseUri.Host)
            {
                log.Warning($"[WRN] Host Violation {pageToVisit}");
                return null;
            }
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
        /// <summary>
        /// Add items to Collected collection
        /// </summary>
        /// <param name="Objects">Objects collected</param>
        /// <param name="CollectedOn">Uri where the Object was found</param>
        public void Collect(IEnumerable<object> Objects, Uri CollectedOn)
        {
            foreach (var o in Objects) Collect(o, CollectedOn);
        }
        /// <summary>
        /// Add item to Collected collection
        /// </summary>
        /// <param name="Object">Object collected</param>
        /// <param name="CollectedOn">Uri where the Object was found</param>
        public void Collect(object Object, Uri CollectedOn)
        {
            lstCollected.Add(new CollectedData(Object: Object, CollectedOn: CollectedOn.ToString()));
        }
        /// <summary>
        /// Get array of Collected Objects
        /// </summary>
        /// <returns></returns>
        public CollectedData[] CollectedItems() { return lstCollected.ToArray(); }

        #region Scheduler

        private void Downloader_FetchFailed(object Sender, FetchFailEventArgs args)
        {
            hExecuted.Add(args.Link.Uri.ToString());
            log.Error($"[ERR] {args.Error.Message} {args.Link}");
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
            if (args.Cancel) log.Information($"[USER CANCEL] {args.Link}");
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
        /// <summary>
        /// All queues finished ?
        /// </summary>
        /// <returns>Whenever the queues finished</returns>
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
        /// <summary>
        /// Current queues size
        /// </summary>
        /// <returns>Returns the size of the queue</returns>
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
            public CollectedData(object Object, string CollectedOn)
            {
                this.Object = Object;
                this.CollectedOn = CollectedOn;
                this.CollectAt = DateTime.Now;
            }

            public object Object { get;  }
            public string CollectedOn { get;  }
            public DateTime CollectAt { get; }
        }
    }
}
