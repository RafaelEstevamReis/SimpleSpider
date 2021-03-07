using System;
using System.Globalization;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Helper
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
            // I really don't like it be a double ... ...
            return DateTime.UnixEpoch.AddSeconds(Timestamp);
        }
        /// <summary>
        /// Converts Unix Epoch from Datetime to number
        /// </summary>
        /// <param name="dateTime">Datetime component to be converted</param>
        /// <returns>>Numeric representation of Unix time</returns>
        public static long UnixEpoch(DateTime dateTime)
        {
            // I really don't like it be a double ... ...
            return (int)(dateTime - DateTime.UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Try convert Text to Decimal using InvariantCulture
        /// </summary>
        /// <param name="Text">Texto to be converted</param>
        /// <param name="OnError">Value returned if conversion fails</param>
        /// <returns>Value converted or OnError value</returns>
        public static decimal ToDecimal(string Text, decimal OnError)
        {
            if (decimal.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v)) return v;
            return OnError;
        }
        /// <summary>
        /// Try convert Text to Int using InvariantCulture
        /// </summary>
        /// <param name="Text">Texto to be converted</param>
        /// <param name="OnError">Value returned if conversion fails</param>
        /// <returns>Value converted or OnError value</returns>
        public static int ToInt(string Text, int OnError)
        {
            if (int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int v)) return v;
            return OnError;
        }
        /// <summary>
        /// Try convert Text to Double using InvariantCulture
        /// </summary>
        /// <param name="Text">Texto to be converted</param>
        /// <param name="OnError">Value returned if conversion fails</param>
        /// <returns>Value converted or OnError value</returns>
        public static double ToDouble(string Text, double OnError)
        {
            if (double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double v)) return v;
            return OnError;
        }
        /// <summary>
        /// Extract digits, comma and dot from input string
        /// </summary>
        /// <param name="Text">Input string</param>
        /// <returns>All digits, commas and dots</returns>
        public static string ExtractNumbers(string Text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var c in Text)
            {
                if (char.IsDigit(c)) sb.Append(c);
                else if(c == ',') sb.Append(c);
                else if(c == '.') sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
