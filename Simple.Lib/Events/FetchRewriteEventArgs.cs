using System;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Represents a method that passes fetch rewrite data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object allowing changing the Uri</param>
    public delegate void FetchRewrite(object Sender, FetchRewriteEventArgs args);

    /// <summary>
    /// Arguments to de FetchRewrite event
    /// </summary>
    public class FetchRewriteEventArgs : EventArgs
    {
        /// <summary>
        /// Construct a FetchRewriteEventArgs
        /// </summary>
        /// <param name="CurrentUri"></param>
        public FetchRewriteEventArgs(Uri CurrentUri)
        {
            this.CurrentUri = CurrentUri;
        }
        /// <summary>
        /// The original Uri added to the queue
        /// </summary>
        public Uri CurrentUri { get; }
        /// <summary>
        /// New Uri to be fetched
        /// </summary>
        public Uri NewUri { get; set; }
        /// <summary>
        /// Defines if should log the rewrite operation
        /// </summary>
        public bool ShowOnLog { get; set; } = true;
    }
}