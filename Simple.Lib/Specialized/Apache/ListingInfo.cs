using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    public class ListingInfo : IEquatable<ListingInfo>
    {
        public bool IsDirectory { get; set; }
        public Uri Parent { get; set; }
        public Uri Uri { get; set; }
        public DateTime LastModified { get; set; }
        public long FileSize { get; set; }
        public string Size { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public bool Equals([AllowNull] ListingInfo other)
        {
            if (other == null) return false;
            return Uri == other.Uri;
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }
        public override string ToString()
        {
            string type = "[DIR]";
            if (!IsDirectory) type = "[FILE]";
            return $"{type} {Uri}";
        }
    }

}
