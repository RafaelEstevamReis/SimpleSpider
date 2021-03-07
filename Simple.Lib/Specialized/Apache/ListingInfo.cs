using System;
using System.Diagnostics.CodeAnalysis;

namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    /// <summary>
    /// Listing item information
    /// </summary>
    public class ListingInfo : IEquatable<ListingInfo>
    {
        /// <summary>
        /// Gets if current item is an directory
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// Uri from where this item were fetched
        /// </summary>
        public Uri Parent { get; set; }
        /// <summary>
        /// Item Uri
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// LastModified column
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Calculated file size
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// Reported file size in human readable form
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// File extension
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Check if argument is the same as this one
        /// </summary>
        public bool Equals([AllowNull] ListingInfo other)
        {
            if (other == null) return false;
            return Uri == other.Uri;
        }
        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
        /// <summary>
        /// String representation of this item
        /// </summary>
        public override string ToString()
        {
            string type = "[DIR]";
            if (!IsDirectory) type = "[FILE]";
            return $"{type} {Uri}";
        }
    }

}
