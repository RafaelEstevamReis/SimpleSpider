namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Represents a method that checks if should fetch data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object allowing cancel the fetching process</param>
    public delegate void ShouldFetch(object Sender, ShouldFetchEventArgs args);

    /// <summary>
    /// Arguments to de ShouldFetch event
    /// </summary>
    public class ShouldFetchEventArgs : FetchEventArgs
    {
        /// <summary>
        /// Reason to not fetch some resource
        /// </summary>
        public enum Reasons
        {
            /// <summary>
            /// This resource was already fetched
            /// </summary>
            AlreadyFetched,
            /// <summary>
            /// User cancelled the process
            /// </summary>
            UserCancelled,
            /// <summary>
            /// User cancelled the process, ignore on Log
            /// </summary>
            UserCancelledSilent,

            /// <summary>
            /// This resource caused an error on previous session
            /// </summary>
            PreviousError,

            /// <summary>
            /// There is no specific reason
            /// </summary>
            None,
        }
        /// <summary>
        /// Instruct the spider to NOT fetch this resource
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// Informs reason to do not fetch
        /// </summary>
        public Reasons Reason { get; set; } = Reasons.None;

        #region Enable Cancel chain

        public ShouldFetchEventArgs CancelAll(string PartialUrl)
        {
            Cancel = true;
            return this;
        }
        public ShouldFetchEventArgs AllowAll(string PartialUrl)
        {
            Cancel = false;
            return this;
        }

        /// <summary>
        /// Sets [Cancel] to True if Link.Uri contains [PartialUrl]
        /// </summary>
        /// <param name="PartialUrl">String to be searched</param>
        public ShouldFetchEventArgs CancelIfContains(string PartialUrl)
        {
            if (Link.Contains(PartialUrl)) Cancel = true;
            return this;
        }
        /// <summary>
        /// Sets [Cancel] to True if Link.Uri DO NOT contains [PartialUrl]
        /// </summary>
        /// <param name="PartialUrl">String to be searched</param>
        public ShouldFetchEventArgs CancelIfNotContains(string PartialUrl)
        {
            if (!Link.Contains(PartialUrl)) Cancel = true;
            return this;
        }

        /// <summary>
        /// Sets [Cancel] to FALSE if Link.Uri contains [PartialUrl]
        /// </summary>
        /// <param name="PartialUrl">String to be searched</param>
        public ShouldFetchEventArgs AllowIfContains(string PartialUrl)
        {
            if (Link.Contains(PartialUrl)) Cancel = false;
            return this;
        }
        /// <summary>
        /// Sets [Cancel] to FALSE if Link.Uri DO NOT contains [PartialUrl]
        /// </summary>
        /// <param name="PartialUrl">String to be searched</param>
        public ShouldFetchEventArgs AllowIfNotContains(string PartialUrl)
        {
            if (!Link.Contains(PartialUrl)) Cancel = false;
            return this;
        }

        public ShouldFetchEventArgs AllowIfIsRoot()
        {
            if (Link.Uri.PathAndQuery == "/") Cancel = false;
            return this;
        }
        public ShouldFetchEventArgs CancelIfIsRoot()
        {
            if (Link.Uri.PathAndQuery == "/") Cancel = true;
            return this;
        }

        #endregion

        /// <summary>
        /// Creates a new ShouldFetchEventArgs
        /// </summary>
        public ShouldFetchEventArgs(Link link)
        {
            this.Link = link;
        }
    }
}