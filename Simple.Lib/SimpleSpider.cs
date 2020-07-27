using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// <example>
    /// <code>
    /// var spider = new SimpleSpider("BooksToScrape", new Uri("http://books.toscrape.com/"));
    /// spider.FetchCompleted += fetchCompleted_items;
    /// spider.Execute();
    /// </code>
    /// </example>
    public sealed partial class SimpleSpider
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
        /// Allow change the Uri just before it is added to the queue
        /// </summary>
        public event FetchRewrite FetchRewrite;

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

        public SpiderData SpiderWorkData { get; private set; }
        private string spiderWorkDataPath;

        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;
        private HashSet<string> hVioleted;

        private List<CollectedData> lstCollected;
        private Logger log; // short to type reference, is accessible through configuration

        /// <summary>
        /// Create a new spider to fetch data from some website
        /// See use examples on the Github page
        /// </summary>
        /// <param name="spiderName">A unique name for this spider. Folder will be created with that name</param>
        /// <param name="baseUri">The base Uri of the website. Pages outside this Host will not be fetched</param>
        /// <param name="params">Additional initialization parameters</param>
        public SimpleSpider(string spiderName, Uri baseUri, InitializationParams @params = null)
        {
            this.SpiderName = spiderName;
            this.BaseUri = baseUri;

            this.Cacher = @params?.Cacher;
            this.Downloader = @params?.Downloader;

            lstCollected = new List<CollectedData>();
            this.Configuration = @params?.ConfigurationPrototype ?? new Configuration();
            initializeConfiguration(spiderName, @params);

            initializeQueues();
            // initialize read-only
            if (Cacher == null) Cacher = new ContentCacher();
            if (Downloader == null) Downloader = new WebClientDownloader();

            initializeFetchers();
            FetchCompleted += fetchCompleted_AutoCollect;
            Parsers = new List<IParserBase>() { new HtmlXElementParser(), new XmlXElementParser(), new JsonParser() };
            if (@params?.Parsers != null) Parsers.AddRange(@params.Parsers);
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

            spiderWorkDataPath = Path.Combine(dataPath.FullName, "privateData.xml");
            if (File.Exists(spiderWorkDataPath))
                SpiderWorkData = Helper.XmlSerializerHelper.DeserializeFromFile<SpiderData>(spiderWorkDataPath);
            else
                SpiderWorkData = new SpiderData();

            if (Configuration.Logger == null)
            {
                Configuration.Spider_LogFile = Path.Combine(spiderPath.FullName, $"{ spiderName }.log");
                log = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Console()
                   .WriteTo.File(Configuration.Spider_LogFile, rollingInterval: RollingInterval.Day)
                   .CreateLogger();
            }
            Configuration.Logger = log;
            log.Information("Initialization complete");
        }

        private void initializeQueues()
        {
            qAdded = new ConcurrentQueue<Link>();
            qCache = new ConcurrentQueue<Link>();
            qDownload = new ConcurrentQueue<Link>();
            hExecuted = new HashSet<string>();
            hVioleted = new HashSet<string>();
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
        private void fetchCompleted_AutoCollect(object Sender, FetchCompleteEventArgs args)
        {
            try
            {
                if (!Configuration.Auto_AnchorsLinks) return;
                if (string.IsNullOrEmpty(args.Html)) return;
                //if (args.Html[0] != '<') return;

                var links = Helper.AnchorHelper.GetAnchors(args.Link.Uri, args.Html);
                // Add the collected links to the queue
                this.AddPage(links, args.Link);
            }
            catch (Exception ex)
            {
                Configuration.Auto_AnchorsLinks = false;
                log.Error(ex, "Failed while auto-collecting links. Auto-collection disabled");
            }
        }

        /// <summary>
        /// Main execution loop, returns once finished
        /// </summary>
        public void Execute()
        {
            Execute(CancellationToken.None);
        }
        /// <summary>
        /// Main execution loop, returns once finished or cancelled
        /// </summary>
        /// <param name="cancellationToken">Cancelation token to prematurely stop</param>
        public void Execute(CancellationToken cancellationToken)
        {
            Cacher.Start();
            Downloader.Start();

            if (QueueSize() == 0) addPage(BaseUri, BaseUri);

            int idleTimeout = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Configuration.Paused)
                {
                    Thread.Sleep(500);
                    continue;
                }

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

             Helper.XmlSerializerHelper.SerializeToFile<SpiderData>(SpiderWorkData, spiderWorkDataPath);

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
                string host = pageToVisit.Host;
                if (!hVioleted.Contains(host)) // ignore the entire domain
                {
                    lock (hVioleted)
                    {
                        hVioleted.Add(host);
                    }
                    log.Warning($"[WRN] Host Violation {pageToVisit}");
                }
                return null;
            }

            if (FetchRewrite != null)
            {
                var ev = new FetchRewriteEventArgs(pageToVisit);
                FetchRewrite(this, ev);
                if (ev.NewUri != null)
                {
                    pageToVisit = ev.NewUri; // Pass HostViolation check
                }
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
            lock (hExecuted)
            {
                hExecuted.Add(args.Link.Uri.ToString());
            }
            log.Error($"[ERR] {args.Error.Message} {args.Link}");
            args.Source = FetchEventArgs.EventSource.Downloader;

            if (args.HttpErrorCode == 404)
            {
                SpiderWorkData.Error404.Add(args.Link.Uri.ToString());
            }

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

        // Should Fetch ?
        private void Cacher_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            args.Source = FetchEventArgs.EventSource.Cacher;
            shouldFetch(Sender, args);
        }

        private void Downloader_ShouldFetch(object Sender, ShouldFetchEventArgs args)
        {
            if (SpiderWorkData.Error404.Contains(args.Link.Uri.ToString()))
            {
                args.Reason = ShouldFetchEventArgs.Reasons.PreviousError;
                args.Cancel = true;
                return;
            }

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
            if (args.Cancel)
            {
                if (args.Reason == ShouldFetchEventArgs.Reasons.None) args.Reason = ShouldFetchEventArgs.Reasons.UserCancelled;

                if (args.Reason == ShouldFetchEventArgs.Reasons.UserCancelled)
                {
                    log.Information($"[USER CANCEL] {args.Link}");
                }
            }
        }
        #endregion

        private void fetchCompleted(FetchCompleteEventArgs args)
        {
            lock (hExecuted) // Hashsets are not threadsafe
            {
                hExecuted.Add(args.Link.Uri.ToString());
            }

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
            // some optimizations to avoid counting
            // if is processing, is not finished
            if (Cacher.IsProcessing) return false;
            if (Downloader.IsProcessing) return false;
            // ig any queue has items, don't count, peek is faser than count
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

        /// <summary>
        /// Prints use instructions on console 
        /// See more on the GitHub project page: https://github.com/RafaelEstevamReis/SimpleSpider
        /// </summary>
        public static void HowToUse_PrintToConsole()
        {
            Console.WriteLine("See full documentation and examples at ");
            Console.WriteLine("   https://github.com/RafaelEstevamReis/SimpleSpider");
            Console.WriteLine();
            Console.WriteLine(Encoding.ASCII.GetString(Files.README));
        }
    }
}
