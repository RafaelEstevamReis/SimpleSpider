using System;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Multi-purpose Fetch event args
    /// </summary>
    /// <typeparam name="T">Type of the argument</typeparam>
    /// <param name="Sender">Event sender</param>
    /// <param name="args">Event arguments</param>
    public delegate void FetchT<T>(object Sender, FetchTEventArgs<T> args);

    /// <summary>
    /// FetchT event EventArgs
    /// </summary>
    public class FetchTEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Link from where the resource was fetched
        /// </summary>
        public Link Link { get; protected set; }
        /// <summary>
        /// Gets or Sets T argument
        /// </summary>
        public T Arg { get; set; }
        /// <summary>
        /// Creates a new instance of FetchTEventArgs
        /// </summary>
        public FetchTEventArgs(Link Link, T Arg)
        {
            this.Link = Link;
            this.Arg = Arg;
        }
    }
}