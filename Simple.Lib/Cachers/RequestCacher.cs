using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Cachers
{
    public class RequestCacher : ICacher
    {
        CancellationTokenSource cancellationToken;
        private DirectoryInfo cacheDir;
        private ConcurrentQueue<Link> queue;
        private Configuration config;
        private Thread[] thread;

        public int ThreadCount
        {
            get;
#if NET5_0
            init;
#else
            private set;
#endif
        }

        public bool IsProcessing { get; private set; }

        public event ShouldUseCache ShouldUseCache;
        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public RequestCacher(int ThreadCount = 4)
        {
            cancellationToken = new CancellationTokenSource();
            this.ThreadCount = ThreadCount;
        }

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

        public void Start()
        {
            if (cancellationToken.Token.IsCancellationRequested) return;

            for (int i = 0; i < thread.Length; i++)
                thread[i].Start();
        }
        public void Stop()
        {
            cancellationToken.Cancel();
        }

        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete)
        {
            var fileInfo = getCacheFileInfo(FetchComplete.Link);
            using (var fs = fileInfo.OpenWrite())
            {
                FetchCompleteEventArgs.SaveFetchResult(FetchComplete, fs);
            }
        }

        public bool HasCache(Link link)
        {
            if (!config.Cache_Enable) return false;

            FileInfo fi = getCacheFileInfo(link.Uri);
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

        private FileInfo getCacheFileInfo(Uri uri)
        {
            return new FileInfo(getCacheFileFullName(uri));
        }
        private string getCacheFileFullName(Uri uri)
        {
            throw new NotImplementedException();
        }


        private void doStuff(object obj)
        {
            while (true)
            {
                Thread.Sleep(10);
                if (cancellationToken.Token.IsCancellationRequested) break;

                try
                {
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
                    else
                    {
                        // empty queue, wait more
                        Thread.Sleep(100);
                    }
                }
                catch (ThreadInterruptedException) { break; }
                catch (Exception ex)
                {
                    config.Logger.Error(ex, "RequestCacher:doStuff_Loop exception");
                    throw;
                }
            }
        }
        private void fetch(Link current)
        {
            IsProcessing = true;
            config.Logger.Information($"[CACHE] {current.Uri.UrlWithoutHost()}");

            current.FetchStart = DateTime.Now;

            var fileInfo = getCacheFileInfo(current);
            FetchCompleteEventArgs fetchArgs;
            using (var fs = fileInfo.OpenRead())
            {
                fetchArgs = FetchCompleteEventArgs.LoadFetchResult(fs);
            }

            current.FetchEnd = DateTime.Now;
            FetchCompleted(this, fetchArgs);

            IsProcessing = false;
        }

    }
}
