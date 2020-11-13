using System;
using System.Collections.Concurrent;
using System.Timers;
using RafaelEstevam.Simple.Spider.Interfaces;

namespace RafaelEstevam.Simple.Spider.Downloaders
{
    /// <summary>
    /// Empty downloader, discards all download requests
    /// </summary>
    public class NullDownloader : IDownloader
    {
        /// <summary>
        /// Discard modes
        /// </summary>
        public enum DiscardModeTypes
        {
            /// <summary>
            /// Downloader should ignore items
            /// </summary>
            Ignore,
            /// <summary>
            /// Downloader should invoke an empty FetchCompleted
            /// </summary>
            CompleteEmpty,
            /// <summary>
            /// Downloader should invoke an empty FetchFailed
            /// </summary>
            Fail,
        }

        private ConcurrentQueue<Link> workQueue;
        private Timer timer;
        /// <summary>
        /// Gets if is processing data
        /// </summary>
        public bool IsProcessing => false;
        /// <summary>
        /// Gets or sets the current queue items DiscardMode
        /// </summary>
        public DiscardModeTypes DiscardMode { get; set; } = DiscardModeTypes.Ignore;

        /// <summary>
        /// Occurs when DiscardMode is set to CompleteEmpty 
        /// </summary>
        public event FetchComplete FetchCompleted;
        /// <summary>
        /// Occurs when DiscardMode is set to Fail
        /// </summary>
        public event FetchFail FetchFailed;
        /// <summary>
        /// Occurs before fetch to check if it should fetch this resource
        /// </summary>
        public event ShouldFetch ShouldFetch;
        /// <summary>
        /// Initialize the downloader
        /// </summary>
        public void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config)
        {
            workQueue = WorkQueue;
            timer = new Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!workQueue.TryDequeue(out Link l)) return;

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
                                                               0,
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
                case DiscardModeTypes.Ignore:
                default:
                    // ignore ... 
                    break;
            }
        }
        /// <summary>
        /// Starts the Downloader operation
        /// </summary>
        public void Start()
        {
            timer.Enabled = true;
        }
        /// <summary>
        /// Stops the Downloader operation
        /// </summary>
        public void Stop()
        {
            timer.Enabled = false;
        }
    }
}
