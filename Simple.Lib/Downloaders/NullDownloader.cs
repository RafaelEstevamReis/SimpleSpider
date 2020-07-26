using System;
using System.Collections.Concurrent;
using System.Timers;
using Net.RafaelEstevam.Spider.Interfaces;

namespace Net.RafaelEstevam.Spider.Downloaders
{
    /// <summary>
    /// Empty downloader, discards all download requests
    /// </summary>
    public class NullDownloader : IDownloader
    {
        public enum DiscardModeTypes
        {
            Ingore,
            CompleteEmpty,
            Fail,
        }

        private ConcurrentQueue<Link> workQueue;
        private Timer timer;

        public bool IsProcessing => false;
        public DiscardModeTypes DiscardMode { get; set; } = DiscardModeTypes.Ingore;

        public event FetchComplete FetchCompleted;
        public event FetchFail FetchFailed;
        public event ShouldFetch ShouldFetch;

        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            workQueue = WorkQueue;
            timer = new Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            workQueue.TryDequeue(out Link l);

            if (ShouldFetch != null)
            {
                var args = new ShouldFetchEventArgs(l);
                ShouldFetch(this, args);
                if (args.Cancel) return; // do not fail/complete
            }

            switch (DiscardMode)
            {
                case DiscardModeTypes.Fail:
                    FetchFailed?.Invoke(this,
                                        new FetchFailEventArgs(l,
                                                               new Exception("NullDownloader:DiscardModeTypes.Fail"),
                                                               new HeaderCollection()));
                    break;
                case DiscardModeTypes.CompleteEmpty:
                    FetchCompleted?.Invoke(this,
                                           new FetchCompleteEventArgs(l,
                                                                      new byte[0],
                                                                      new HeaderCollection(),
                                                                      new HeaderCollection()));
                    break;
                case DiscardModeTypes.Ingore:
                default:
                    // ignore ... 
                    break;
            }
        }

        public void Start()
        {
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }
    }
}
