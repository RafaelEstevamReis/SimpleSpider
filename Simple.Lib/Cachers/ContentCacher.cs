using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Cachers
{
    public class ContentCacher : ICacher
    {
        private const int ThreadCount = 4;

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
            thread = new Thread[ThreadCount];
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
            if (!config.Cache_Enable) return false;

            var fi = new FileInfo(getCacheFileFullName(uri));
            if (!fi.Exists) return false;

            if (config.Cache_Lifetime != null)
            {
                if (DateTime.Now - fi.LastWriteTime > config.Cache_Lifetime.Value) return false;
            }

            return true;
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
                        var textContent = Encoding.UTF8.GetString(bytes, 0, 64);

                        // don't know, so we guess
                        string cType = null;
                        if (textContent[0] == '{' && textContent.Contains(":")) cType = "application/json";
                        else
                        {
                        }

                        var rHrd = new List<KeyValuePair<string, string>>();
                        if (cType != null)
                        {
                            rHrd.Add(new KeyValuePair<string, string>("Content-Type", cType));
                        }

                        FetchCompleted(this, new FetchCompleteEventArgs(current, 
                                                                        bytes, 
                                                                        new KeyValuePair<string, string>[0], 
                                                                        rHrd.ToArray()));

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
