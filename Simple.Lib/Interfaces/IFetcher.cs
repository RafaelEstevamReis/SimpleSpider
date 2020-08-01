using System.Collections.Concurrent;

namespace Net.RafaelEstevam.Spider.Interfaces
{
    /// <summary>
    /// Represents a module that Fetch some resource
    /// </summary>
    public interface IFetcher
    {
        /// <summary>
        /// Occurs when a fetch complete
        /// </summary>
        event FetchComplete FetchCompleted;
        /// <summary>
        /// Occurs when a fetch fail
        /// </summary>
        event FetchFail FetchFailed;
        /// <summary>
        ///  Occurs before fetch to check if a fetch can occur
        /// </summary>
        event ShouldFetch ShouldFetch;

        /// <summary>
        /// Pass spider data to initialize the module
        /// </summary>
        /// <param name="WorkQueue">The queue this module will use to fetch data from</param>
        /// <param name="Config">Current configuration</param>
        void Initialize(ConcurrentQueue<Link> WorkQueue, Configuration Config);

        /// <summary>
        /// The module is busy processing some resource
        /// </summary>
        bool IsProcessing { get; }

        /// <summary>
        /// Starts the module
        /// </summary>
        void Start();
        /// <summary>
        /// Stops the module
        /// </summary>
        void Stop();
    }
}
