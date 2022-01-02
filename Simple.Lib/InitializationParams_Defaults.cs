using RafaelEstevam.Simple.Spider.Cachers;
using RafaelEstevam.Simple.Spider.Downloaders;

namespace RafaelEstevam.Simple.Spider
{
    public partial class InitializationParams
    {
        /// <summary>
        /// Frozen in time default: Minimal features, nothing enabled
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        public static InitializationParams Default000(int DownloadDelay = 5000)
        {
            // Have non-changing defaults helps with not breaking stuff
            return new InitializationParams()
                // Future change in defaults
                //will not affect this template
                .SetCacher(new ContentCacher())
                .SetDownloader(new WebClientDownloader())
                .SetLinkCollector(new LinkProcessors.SimpleProcessor())
                .SetStorage(null)
                .SetConfig(c => c.Enable_Caching()
                                 .Set_CachingNoLimit()
                                 .Disable_Cookies()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Disable_AutoRewriteRemoveFragment()
                                 .Disable_AutoAnchorsLinks());
        }

        /// <summary>
        /// Frozen in time default: ContentCacher, WebClientDownloader, NoLimitCaching, and AutoAnchorsLinks enabled
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        public static InitializationParams Default001(int DownloadDelay = 5000)
        {
            // Have non-changing defaults helps with not breaking stuff
            //but still have a good start point

            return new InitializationParams()
                // Set stable fetchers, future change in defaults
                //will not affect this template
                .SetCacher(new ContentCacher()) // more stable for the time (the only one, but still)
                .SetDownloader(new WebClientDownloader())
                .SetLinkCollector(new LinkProcessors.SimpleProcessor())
                .SetStorage(null)
                .SetConfig(c => c.Disable_AutoRewriteRemoveFragment()
                                 .Enable_Caching()
                                 .Set_CachingNoLimit()
                                 .Disable_Cookies()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Enable_AutoAnchorsLinks());
        }


        /// <summary>
        /// Frozen in time default: ContentCacher, HttpClientDownloader with headers, NoLimitCaching, AutoAnchorsLinks enabled, and rewrites fragments
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        public static InitializationParams Default002(int DownloadDelay = 5000)
        {
            // Have non-changing defaults helps with not breaking stuff
            //but still have a good start point

            return new InitializationParams()
                // Set stable fetchers, future change in defaults
                //will not affect this template
                .SetCacher(new ContentCacher()) // more stable for the time (the only one, but still)
                .SetDownloader(new HttpClientDownloader(true))
                .SetLinkCollector(new LinkProcessors.SimpleProcessor())
                .SetStorage(null)
                .SetConfig(c => c.Enable_AutoRewriteRemoveFragment()
                                 .Enable_Caching()
                                 .Set_CachingNoLimit()
                                 .Disable_Cookies()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Enable_AutoAnchorsLinks());
        }

        /// <summary>
        /// Frozen in time default: RequestCacher, HttpClientDownloader with headers, NoLimitCaching, AutoAnchorsLinks enabled, and rewrites fragments
        /// </summary>
        /// <param name="DownloadDelay">Config.DownloadDelay in milliseconds</param>
        public static InitializationParams Default003(int DownloadDelay = 5000)
        {
            // Have non-changing defaults helps with not breaking stuff
            //but still have a good start point

            return new InitializationParams()
                // Set stable fetchers, future change in defaults
                //will not affect this template
                .SetCacher(new RequestCacher())
                .SetDownloader(new HttpClientDownloader(true))
                .SetLinkCollector(new LinkProcessors.SimpleProcessor())
                .SetStorage(null)
                .SetConfig(c => c.Enable_AutoRewriteRemoveFragment()
                                 .Enable_Caching()
                                 .Set_CachingNoLimit()
                                 .Disable_Cookies()
                                 .Set_DownloadDelay(DownloadDelay)
                                 .Enable_AutoAnchorsLinks());
        }
    }
}
