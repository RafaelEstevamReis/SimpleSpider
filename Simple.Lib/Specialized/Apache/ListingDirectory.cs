using System.Collections.Generic;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    /// <summary>
    /// Represents a directory listed
    /// </summary>
    public class ListingDirectory
    {
        /// <summary>
        /// Gets the underlying ListinInfo object
        /// </summary>
        public ListingInfo Entity { get; }
        /// <summary>
        /// Get sub directories
        /// </summary>
        public List<ListingDirectory> Directories { get; }
        /// <summary>
        /// Gets all files in this directory
        /// </summary>
        public List<ListingFile> Files { get; }
        
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ListingDirectory(ListingInfo entity)
        {
            Directories = new List<ListingDirectory>();
            Files = new List<ListingFile>();
            Entity = entity;
        }
        /// <summary>
        /// Gets if this directory has files
        /// </summary>
        public bool HasFiles => Files.Count > 0;
        /// <summary>
        /// Gets if this directory has sub directories
        /// </summary>
        public bool HasDirectories => Directories.Count > 0;
        /// <summary>
        /// Gets if this directory has any files of sub directories
        /// </summary>
        public bool IsEmpty => !(HasFiles || HasDirectories);

        /// <summary>
        /// List all directories descendants of this one
        /// </summary>
        public IEnumerable<ListingDirectory> GetAllDescendants()
        {
            return Directories.SelectMany(d => d.GetAllDescendantsAndSelf());
        }
        /// <summary>
        /// List all directories descendants including this one
        /// </summary>
        public IEnumerable<ListingDirectory> GetAllDescendantsAndSelf()
        {
            yield return this;
            foreach (var d in GetAllDescendants()) yield return d;
        }
        /// <summary>
        /// String representation of current object
        /// </summary>
        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}
