namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    public class ListingOptions
    {
        public bool NoParent { get; set; } = true;
        public bool AllowCaching { get; set; } = true;
        public ShouldFetch ShouldFetch { get; set; }
    }

}
