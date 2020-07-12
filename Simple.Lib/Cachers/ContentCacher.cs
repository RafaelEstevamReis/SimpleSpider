using Net.RafaelEstevam.Spider.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Net.RafaelEstevam.Spider.Cachers
{
    public class ContentCacher : ICacher
    {
        private DirectoryInfo cacheDir;
        private ConcurrentQueue<Link> queue;
        private Configuration config;
        private Thread[] thread;

        public bool IsProcessing { get; private set; }

        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            var cachePath = new DirectoryInfo(Path.Combine(Config.SpiderDirectory.FullName, "CACHE"));
            if (!cachePath.Exists) cachePath.Create();
            cacheDir = cachePath;

            queue = WorkQueue;
            config = Config;
            thread = new Thread[4];
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i] = new Thread(doStuff);
                thread[i].Name = $"thdCache[{i}]";
            }
        }

        public void GenerateCacheFor(FetchCompleteEventArgs FetchComplete)
        {
            File.WriteAllBytes(getCacheFileFullName(FetchComplete.Link), FetchComplete.Result);
        }

        public bool HasCache(Uri uri)
        {
            return File.Exists(getCacheFileFullName(uri));
        }

        bool running = false;
        public void Start()
        {
            running = true;

            for (int i = 0; i < thread.Length; i++)
                thread[i].Start();
        }

        public void Stop()
        {
            running = false;
            for (int i = 0; i < thread.Length; i++)
                thread[i].Interrupt();
        }

        private void doStuff(object obj)
        {
            while (running)
            {
                try
                {
                    Thread.Sleep(10);

                    if (queue.TryDequeue(out Link current))
                    {
                        var args = new ShouldFetchEventArgs(current);
                        ShouldFetch(this, args);
                        if (args.Cancel) continue;

                        IsProcessing = true;
                        Console.WriteLine($"[CACHE] {current.Uri}");

                        // load file
                        var bytes = File.ReadAllBytes(getCacheFileFullName(current));
                        FetchCompleted(this, new FetchCompleteEventArgs(current, bytes, new KeyValuePair<string, string>[0]));

                        IsProcessing = false;
                    }
                    else Thread.Sleep(100);
                }
                catch (ThreadInterruptedException) { }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private string getCacheFileFullName(Uri uri)
        {
            return Path.Combine(cacheDir.FullName, getCacheFileName(uri));
        }
        private static string getCacheFileName(Uri uri)
        {
            var remove = Path.GetInvalidFileNameChars();
            string url = uri.AbsolutePath;
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
