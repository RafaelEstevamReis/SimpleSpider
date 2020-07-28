using System;

namespace Net.RafaelEstevam.Spider
{
    public sealed partial class SimpleSpider
    {
        /// <summary>
        /// Class to store volatile data during the process
        /// </summary>
        public class CollectedData
        {
            /// <summary>
            /// New data object to be stored
            /// </summary>
            /// <param name="Object">Object to be stored</param>
            /// <param name="CollectedOn">Where was it found</param>
            public CollectedData(object Object, string CollectedOn)
            {
                this.Object = Object;
                this.CollectedOn = CollectedOn;
                this.CollectAt = DateTime.Now;
            }
            /// <summary>
            /// Object stored
            /// </summary>
            public object Object { get;  }
            /// <summary>
            /// Url of where was it found
            /// </summary>
            public string CollectedOn { get;  }
            /// <summary>
            /// DateTime of when was it found (in fact, stored)
            /// </summary>
            public DateTime CollectAt { get; }
        }
    }
}
