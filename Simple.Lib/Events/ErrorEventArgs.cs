using System;

namespace RafaelEstevam.Simple.Spider
{
    public delegate void Error(object Sender, ErrorEventArgs args);
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        /// <summary>
        /// Source from this event
        /// </summary>
        public FetchEventArgs.EventSource Source { get; internal set; }
    }
}
