using System;

namespace RafaelEstevam.Simple.Spider
{
    /// <summary>
    /// Represents a method that passes an error info
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object containing error data</param>
    public delegate void Error(object Sender, ErrorEventArgs args);
    /// <summary>
    /// Arguments to de Error event
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Exception data
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Source from this event
        /// </summary>
        public FetchEventArgs.EventSource Source { get; internal set; }
    }
}
