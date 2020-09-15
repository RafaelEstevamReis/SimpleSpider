using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Interfaces;
using Net.RafaelEstevam.Spider.Parsers;
using Newtonsoft.Json;
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
        /// <summary>
        /// Current storage engine
        /// </summary>
        public IStorage Storage { get; }

        /// <summary>
        /// Spider private work data, mess with care
        /// </summary>
        public SpiderData SpiderWorkData { get; private set; }
        private string spiderWorkDataPath;

        private ConcurrentQueue<Link> qAdded;
        private ConcurrentQueue<Link> qCache;
        private ConcurrentQueue<Link> qDownload;
        private HashSet<string> hExecuted;
        private HashSet<string> hDispatched;
        private HashSet<string> hViolated;
        private List<Link> lCompleted;

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
            if (Downloader == null) Downloader = new HttpClientDownloader();

            initializeFetchers();
            FetchCompleted += fetchCompleted_AutoCollect;
            FetchRewrite += fetchRewrite_AutoRewrite;

            Parsers = new List<IParserBase>();
            if (@params?.Parsers != null) Parsers.AddRange(@params.Parsers);

            if (@params?.StorageEngine != null)
            {
                Storage = @params.StorageEngine;
                Storage.Initialize(Configuration);
            }

            logInitialStatus();
        }
        private void initializeConfiguration(string spiderName, InitializationParams init)
        {
            var dir = init?.SpiderDirectory;
            if (dir == null) dir = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory;

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
        }
        private void initializeQueues()
        {
            qAdded = new ConcurrentQueue<Link>();
            qCache = new ConcurrentQueue<Link>();
            qDownload = new ConcurrentQueue<Link>();
            hExecuted = new HashSet<string>();
            hDispatched = new HashSet<string>();
            hViolated = new HashSet<string>();
            lCompleted = new List<Link>();
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

        private void logInitialStatus()
        {
            log.Information("Initialization complete");
            log.Information($" > Name:       {SpiderName}");
            log.Information($" > BaseUri:    {BaseUri}");
            log.Information($" > Cacher:     {Cacher}");
            log.Information($" > Downloader: {Downloader}");
            log.Information($" > Directory:  {Configuration.SpiderDirectory}");
        }

        private void fetchCompleted_AutoCollect(object Sender, FetchCompleteEventArgs args)
        {
            try
            {
                if (!Configuration.Auto_AnchorsLinks) return;
                if (string.IsNullOrEmpty(args.Html)) return;

                var links = AnchorHelper.GetAnchors(args.Link.Uri, args.Html);
#if DEBUG
                var arr = links.ToArray(); // enumerate to test results
#endif

                // Add the collected links to the queue
                AddPages(links, args.Link);
            }
            catch (Exception ex)
            {
                Configuration.Auto_AnchorsLinks = false;
                log.Error(ex, "Failed while auto-collecting links. Auto-collection disabled");
            }
        }
        private void fetchRewrite_AutoRewrite(object Sender, FetchRewriteEventArgs args)
        {
            try
            {
                if (!Configuration.Auto_RewriteRemoveFragment) return;
                if (args.CurrentUri.HasFragment())
                {
                    args.NewUri = args.CurrentUri.RemoveFragment();
                    args.ShowOnLog = false;
                }
            }
            catch (Exception ex)
            {
                Configuration.Auto_RewriteRemoveFragment = false;
                log.Error(ex, "Failed while auto-removing fragments. Auto-removing disabled");
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
        /// <param name="cancellationToken">Cancellation token to prematurely stop</param>
        public void Execute(CancellationToken cancellationToken)
        {
            bool running = true;
            Cacher.Start();
            Downloader.Start();
            Storage?.LoadData();

            if (QueueSize() == 0) addPage(BaseUri, BaseUri);

            int idleTimeout = 0;
            int saveCount = 0;
            while (running)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    log.Information("Cancellation was Requested");
                    break;
                }

                if (Configuration.Paused)
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (workQueue()) continue;

                Thread.Sleep(100);
                if (QueueFinished())
                {
                    if (idleTimeout++ > 20)
                    {
                        break;
                    }
                }
                else
                {
                    idleTimeout = 0;
                }

                if (saveCount++ > 1000) // Each loop is 0,1 or 0,5s, 1 min => ~600
                {
                    saveCount = 0;
                    saveSpiderData(true);
                }
            }

            saveSpiderData(false);
            Cacher.Stop();
            Downloader.Stop();
        }
        private void saveSpiderData(bool autoSave)
        {
            lock (spiderWorkDataPath)
            {
                try
                {
                    XmlSerializerHelper.SerializeToFile<SpiderData>(SpiderWorkData, spiderWorkDataPath);
                    log.Debug("Spider internal data saved");
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Failed to save spider internal data");
                }
            }
            Storage?.SaveData(autoSave);
        }

        private bool workQueue()
        {
            if (qAdded.TryDequeue(out Link lnk))
            {
                if (alreadyExecuted(lnk.Uri)) return true;

                if(hDispatched.Contains(lnk.Uri.ToString())) return true;
                hDispatched.Add(lnk.Uri.ToString());

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
        public Link[] AddPages(IEnumerable<Uri> PagesToVisit, Uri SourcePage)
        {
            return addPages(PagesToVisit, SourcePage)
                .ToArray(); // Force enumeration
        }
        private IEnumerable<Link> addPages(IEnumerable<Uri> PagesToVisit, Uri SourcePage)
        {
            foreach (var p in PagesToVisit)
            {
                yield return AddPage(p, SourcePage);
            }
        }
        /// <summary>
        /// Old method with a typo (singular) on name. Use [AddPages] (with 's') instead
        /// Will be removed soon
        /// </summary>
        [Obsolete]
        public void AddPage(IEnumerable<Uri> PagesToVisit, Uri SourcePage)
        {
            AddPages(PagesToVisit, SourcePage);
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
                if (!hViolated.Contains(host)) // ignore the entire domain
                {
                    lock (hViolated)
                    {
                        hViolated.Add(host);
                    }
                    log.Warning($"[WRN] Host Violation {pageToVisit}");
                }
                return null;
            }

            var lnk = new Link(pageToVisit, sourcePage);

            if (FetchRewrite != null)
            {
                var ev = new FetchRewriteEventArgs(pageToVisit);
                FetchRewrite(this, ev);
                // Default Uri Equality ignores Fragment
                if (ev.NewUri != null && ev.NewUri.ToString() != pageToVisit.ToString())
                {
                    if (ev.ShowOnLog) log.Information($"[REW] {pageToVisit} -> {ev.NewUri}");
                    lnk.ResourceRewritten(ev.NewUri);
                }
            }

            if (SpiderWorkData.Moved301.ContainsKey(lnk.Uri.ToString()))
            {
                string newUri = SpiderWorkData.Moved301[lnk.Uri.ToString()];
                lnk.ResourceMoved(new Uri(newUri));

                if (alreadyExecuted(lnk.Uri)) return null; 
                //log.Information($"[A301] {pageToVisit} -> {newUri}");
            }

            if (alreadyExecuted(lnk.Uri)) return null;

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
        /// Add items to the volatile collection. Don't forget to retrieve them later with CollectedItems()
        /// </summary>
        /// <param name="Objects">Objects collected</param>
        /// <param name="CollectedOn">Uri where the Object was found</param>
        public void Collect(IEnumerable<dynamic> Objects, Uri CollectedOn)
        {
            foreach (var o in Objects) Collect(o, CollectedOn);
        }
        /// <summary>
        /// Add item to the volatile collection. Don't forget to retrieve them later with CollectedItems()
        /// </summary>
        /// <param name="Object">Object collected</param>
        /// <param name="CollectedOn">Uri where the Object was found</param>
        public void Collect(dynamic Object, Uri CollectedOn)
        {
            lstCollected.Add(new CollectedData(Object: Object, CollectedOn: CollectedOn.ToString()));
        }
        /// <summary>
        /// Get array with all Collected Objects
        /// </summary>
        public CollectedData[] CollectedItems() { return lstCollected.ToArray(); }
        /// <summary>
        /// Writes all Collected Objects to the stream
        /// </summary>
        /// <param name="writer">Stream to write to</param>
        /// <param name="IncludeMetadata">Defines if should include the metadata. Set to false to export data only</param>
        public void SaveCollectedItems(StreamWriter writer, bool IncludeMetadata = true)
        {
            using var jw = new JsonTextWriter(writer);
            var serializer = new JsonSerializer();

            if (IncludeMetadata) serializer.Serialize(jw, lstCollected);
            else serializer.Serialize(jw, lstCollected.Select(c => c.Object));
        }

        #region Scheduler callbacks

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
            try
            {
                Cacher.GenerateCacheFor(args);
            }
            catch (IOException ex)
            {
                log.Error(ex, "Failed to generate cache with an IO Exception. Spider will be paused");
                Configuration.Paused = true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to generate cache");
            }
            args.Source = FetchEventArgs.EventSource.Downloader;

            if (args.Link.MovedUri != null)
            {
                SpiderWorkData.Moved301[args.Link.MovedUri.ToString()] = args.Link.Uri.ToString();
            }

            fetchCompleted(args);
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
        private void fetchCompleted(FetchCompleteEventArgs args)
        {
            lock (hExecuted) // Hashsets are not threadsafe
            {
                hExecuted.Add(args.Link.Uri.ToString());
                if (args.Link.MovedUri != null)
                {
                    hExecuted.Add(args.Link.MovedUri.ToString());
                }
                lCompleted.Add(args.Link);
            }

            // Main CallBack
            if (FetchCompleted != null)
            {
                foreach (FetchComplete e in FetchCompleted.GetInvocationList())
                {
                    try
                    {
                        e.DynamicInvoke(this, args);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Error on FetchComplete event");
                    }
                }
            }

            // Additional Link callback
            args.Link.FetchCompleteCallBack?.Invoke(this, args);

            // Parsers
            var contentType = args.ResponseHeaders.FirstOrDefault(h => h.Key == "Content-Type");
            if (contentType.Value == null)
            {
                // try to guess
                var textContent = System.Text.Encoding.ASCII.GetString(args.Result.Take(128).ToArray());
                if (!string.IsNullOrEmpty(textContent))
                {
                    if (textContent[0] == '{' && textContent.Contains(":"))
                        contentType = new KeyValuePair<string, string>("Content-Type", "application/json");
                    if (textContent[0] == '<' && textContent.ToLower().Contains("html"))
                        contentType = new KeyValuePair<string, string>("Content-Type", "text/html");
                }
            }
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
        #endregion

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
            // ig any queue has items, don't count, peek is faster than count
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
        /// Returns an array with links of all completed fetchs
        /// </summary>
        /// <returns>An array of Links</returns>
        public Link[] AllCompletedLinks()
        {
            return lCompleted.ToArray();
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
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(Files.README));
        }
    }
}
