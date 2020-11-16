using System;

namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Arguments to de Fetch event
    /// </summary>
    public class FetchEventArgs : EventArgs
    {
        /// <summary>
        /// Module that fired the event
        /// </summary>
        public enum EventSource
        {
            /// <summary>
            /// Event initiated by the Cacher
            /// </summary>
            Cacher,
            /// <summary>
            /// Event initiated by the Downloader
            /// </summary>
            Downloader,
            /// <summary>
            /// Event initiated by the Scheduler
            /// </summary>
            Scheduler,
        }

        /// <summary>
        /// The request headers used to query
        /// </summary>
        public HeaderCollection RequestHeaders { get; protected set; }
        /// <summary>
        /// Link from where the resource was fetched
        /// </summary>
        public Link Link { get; protected set; }
        /// <summary>
        /// Source from this event
        /// </summary>
        public EventSource Source { get; internal set; }
    }
}