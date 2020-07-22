using System;
using System.Collections.Generic;
using System.Text;

namespace Net.RafaelEstevam.Spider.Helper
{
    public static class ConversionHelper
    {
        public static DateTime UnixEpoch(long Timestamp)
        {
            // I realy don't like it be a double ... ...
            return DateTime.UnixEpoch.AddSeconds(Timestamp);
        }
        public static long UnixEpoch(DateTime dateTime)
        {
            // I realy don't like it be a double ... ...
            return (int)(dateTime - DateTime.UnixEpoch).TotalSeconds;
        }
    }
}
