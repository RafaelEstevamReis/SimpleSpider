using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Helper to convert stuff frequently found on websites or API responses
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        /// Converts Unix Epoch from number to Datetime
        /// </summary>
        /// <param name="Timestamp">Numeric representation of Unix time</param>
        /// <returns>Datetime converted</returns>
        public static DateTime UnixEpoch(long Timestamp)
        {
            // I realy don't like it be a double ... ...
            return DateTime.UnixEpoch.AddSeconds(Timestamp);
        }
        /// <summary>
        /// Converts Unix Epoch from Datetime to number
        /// </summary>
        /// <param name="dateTime">Datetime component to be converted</param>
        /// <returns>>Numeric representation of Unix time</returns>
        public static long UnixEpoch(DateTime dateTime)
        {
            // I realy don't like it be a double ... ...
            return (int)(dateTime - DateTime.UnixEpoch).TotalSeconds;
        }
    }
}
