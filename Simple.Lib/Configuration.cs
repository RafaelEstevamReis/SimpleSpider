using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider
{
    public class Configuration
    {
        /// <summary>
        /// Time to wait in miliseconds between downloads
        /// </summary>
        public int DownloadDelay { get; internal set; }
    }
}
