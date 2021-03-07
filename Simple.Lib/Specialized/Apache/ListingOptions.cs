namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    /// <summary>
    /// Options for apache listing 
    /// </summary>
    public class ListingOptions
    {
        /// <summary>
        /// Do not follow links up
        /// </summary>
        public bool NoParent { get; set; } = true;
        /// <summary>
        /// Enables caching
        /// </summary>
        public bool AllowCaching { get; set; } = true;
        /// <summary>
        /// Callback the choose if a directory should be fetched 
        /// </summary>
        public ShouldFetch ShouldFetch { get; set; }
    }

}
