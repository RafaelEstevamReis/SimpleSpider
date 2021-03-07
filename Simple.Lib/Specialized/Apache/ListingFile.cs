using System;

namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    /// <summary>
    /// Represents a file listed
    /// </summary>
    public class ListingFile : ListingInfo
    {
        private ListingFile() { }
        /// <summary>
        /// Creates a new ListingFile instance from a ListingInfo
        /// </summary>
        public static ListingFile Create(ListingInfo entity)
        {
            if (entity.IsDirectory) throw new ArgumentException("Must be a file");

            return new ListingFile()
            {
                IsDirectory = false,
                Uri = entity.Uri,
                Parent = entity.Parent,
                FileName = entity.FileName,
                FileExtension = entity.FileExtension,
                FileSize = entity.FileSize,
                Size = entity.Size,
                LastModified = entity.LastModified,

            };
        }
        /// <summary>
        /// String representation of current file
        /// </summary>
        public override string ToString()
        {
            return $"{FileName} [{Size}]";
        }
    }
}
