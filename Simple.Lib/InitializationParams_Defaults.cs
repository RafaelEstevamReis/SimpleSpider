using Net.RafaelEstevam.Spider.Cachers;
using Net.RafaelEstevam.Spider.Downloaders;

namespace Net.RafaelEstevam.Spider
{
    public partial class InitializationParams
    {
        /// <summary>
        /// Frozen in time default: ContentCacher, WebClientDownloader, NoLimitCaching, and AutoAnchorsLinks enabled
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        /// <returns></returns>
        public static InitializationParams Default001(int DownloadDelay = 5000)
        {
            // Have non-changing defaults helps with not breaking stuff
            //but still have a good start point

            return new InitializationParams()
                // Set stable fetchers, future change in defaults
                //will not affect this template
                .SetCacher(new ContentCacher()) // more stable for the time (the only one, but still)
                .SetDownloader(new WebClientDownloader())
                .SetConfig(c => c.Enable_Caching()
                                 .Disable_Cookies()
                                 .Set_CachingNoLimit()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Enable_AutoAnchorsLinks());
        }
    }
}
