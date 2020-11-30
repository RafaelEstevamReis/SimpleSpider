using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using RafaelEstevam.Simple.Spider.Events;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Cachers
{
    /// <summary>
    /// Simple cacher, stores the content of the resource fetched locally
    /// </summary>
    public class ContentCacher : ICacher
    {
        private const int ThreadCount = 4;

        private DirectoryInfo cacheDir;
        private ConcurrentQueue<Link> queue;
        private Configuration config;
        private Thread[] thread;
        CancellationTokenSource cancellationToken;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ContentCacher()
        {
            cancellationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets if is processing
        /// </summary>
        public bool IsProcessing { get; private set; }

        /// <summary>
        /// Occurs when fetch is complete 
        /// </summary>
        public event FetchComplete FetchCompleted;
        /// <summary>
        /// Occurs when fetch fails
        /// </summary>
        public event FetchFail FetchFailed;
        /// <summary>
        /// Occurs before fetch to check if it should fetch this resource
        /// </summary>
        public event ShouldFetch ShouldFetch;
        /// <summary>
        /// Occurs before fetch to check if the cache can be used
        /// </summary>
        public event ShouldUseCache ShouldUseCache;

        /// <summary>
        /// Initialize the cacher
        /// </summary>
        /// <param name="WorkQueue">The queue to be used</param>
        /// <param name="Config">The configuration to be used</param>
        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            var cachePath = new DirectoryInfo(Path.Combine(Config.SpiderDirectory.FullName, "CACHE"));
            if (!cachePath.Exists) cachePath.Create();
            cacheDir = cachePath;

            queue = WorkQueue;
            config = Config;
            thread = new Thread[ThreadCount];
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i] = new Thread(doStuff);
                thread[i].Name = $"thdCache[{i}]";
            }
        }

        /// <summary>
        /// Instructs the cacher to generate a cache for the new resource
        /// </summary>
        /// <param name="FetchComplete">Fetch data to save</param>
        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete)
        {
            File.WriteAllBytes(getCacheFileFullName(FetchComplete.Link), FetchComplete.Result);
        }
        /// <summary>
        /// Gets if a cache exists
        /// </summary>
        /// <param name="link">Uri to check for</param>
        /// <returns>True if has a cache, False otherwise</returns>
        public bool HasCache(Link link)
        {
            if (!config.Cache_Enable) return false;

            var fi = new FileInfo(getCacheFileFullName(link.Uri));
            if (!fi.Exists) return false;

            if (config.Cache_Lifetime != null)
            {
                if (DateTime.Now - fi.LastWriteTime > config.Cache_Lifetime.Value) return false;
            }

            if (ShouldUseCache != null)
            {
                var args = new ShouldUseCacheEventArgs(link)
                {
                    PageCacheDate = fi.LastWriteTime,
                    Source = FetchEventArgs.EventSource.Cacher,
                };

                ShouldUseCache(this, args);

                if (args.Cancel) return false;
            }

            return true;
        }

        bool running = false;
        /// <summary>
        /// Starts the Cacher operation
        /// </summary>
        public void Start()
        {
            running = true;

            for (int i = 0; i < thread.Length; i++)
                thread[i].Start();
        }
        /// <summary>
        /// Stops the Cacher operation
        /// </summary>
        public void Stop()
        {
            running = false;
            cancellationToken.Cancel();
            //for (int i = 0; i < thread.Length; i++)
            //    thread[i].Interrupt();
        }

        private void doStuff(object obj)
        {
            while (running)
            {
                try
                {
                    Thread.Sleep(10);

                    if (cancellationToken.Token.IsCancellationRequested) break;

                    if (config.Paused || config.Paused_Cacher)
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    if (queue.TryDequeue(out Link current))
                    {
                        var args = new ShouldFetchEventArgs(current);
                        ShouldFetch(this, args);
                        if (args.Cancel) continue;

                        try
                        {
                            fetch(current);
                        }
                        catch (Exception ex)
                        {
                            FetchFailed(this, new FetchFailEventArgs(current, 0, ex, new HeaderCollection()));
                        }
                    }
                    else Thread.Sleep(100);
                }
                catch (ThreadInterruptedException) { }
                catch (Exception ex)
                {
                    config.Logger.Error(ex, "ContentCacher:doStuff_Loop exception");
                    throw;
                }
            }
        }

        private void fetch(Link current)
        {
            IsProcessing = true;
            config.Logger.Information($"[CACHE] {current.Uri.UrlWithoutHost()}");

            current.FetchStart = DateTime.Now;
            // load file
            var bytes = File.ReadAllBytes(getCacheFileFullName(current));
            var textContent = Encoding.ASCII.GetString(bytes.Take(128).ToArray());

            // don't know, so we guess
            string cType = null;
            if (!string.IsNullOrEmpty(textContent))
            {
                if (textContent[0] == '{' && textContent.Contains(":")) cType = "application/json";
                if (textContent[0] == '<' && textContent.ToLower().Contains("html")) cType = "text/html";
                else
                {
                }
            }

            var rHrd = new List<KeyValuePair<string, string>>();
            if (cType != null)
            {
                rHrd.Add(new KeyValuePair<string, string>("Content-Type", cType));
            }
            current.FetchEnd = DateTime.Now;
            FetchCompleted(this, new FetchCompleteEventArgs(current,
                                                            bytes,
                                                            new HeaderCollection(),
                                                            new HeaderCollection(rHrd)));

            IsProcessing = false;
        }

        private string getCacheFileFullName(Uri uri)
        {
            return Path.Combine(cacheDir.FullName, getCacheFileName(uri));
        }
        private static string getCacheFileName(Uri uri)
        {
            var remove = Path.GetInvalidFileNameChars();
            string url = uri.PathAndQuery;
            foreach (var c in remove)
            {
                if (url.Contains(c))
                {
                    url = url.Replace(c, '_');
                }
            }
            return $"{url}.cache";
        }
    }
}
