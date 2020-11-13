using RafaelEstevam.Simple.Spider.Cachers;
using RafaelEstevam.Simple.Spider.Downloaders;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.InitParamsTests
{
    public class InitParams_DefaultsTests
    {
        [Fact]
        public void InitializationParams_Default000()
        {
            int delay = 4321;
            var init = InitializationParams.Default000(delay);

            Assert.IsType<ContentCacher>(init.Cacher);
            Assert.IsType<WebClientDownloader>(init.Downloader);
            Assert.Empty(init.Parsers);
            Assert.Null(init.StorageEngine);
            Assert.Null(init.SpiderDirectory);

            var cfg = init.ConfigurationPrototype;
            Assert.False(cfg.Auto_RewriteRemoveFragment);
            Assert.True(cfg.Cache_Enable);
            Assert.Null(cfg.Cache_Lifetime);
            Assert.Equal(delay, cfg.DownloadDelay);
            Assert.False(cfg.Cookies_Enable);
            Assert.False(cfg.Paused);
            Assert.False(cfg.Paused_Cacher);
            Assert.False(cfg.Paused_Downloader);
            Assert.False(cfg.Auto_AnchorsLinks);
        }

        [Fact]
        public void InitializationParams_Default001()
        {
            int delay = 4321;
            var init = InitializationParams.Default001(delay);

            Assert.IsType<ContentCacher>(init.Cacher);
            Assert.IsType<WebClientDownloader>(init.Downloader);
            Assert.Empty(init.Parsers);
            Assert.Null(init.StorageEngine);
            Assert.Null(init.SpiderDirectory);

            var cfg = init.ConfigurationPrototype;
            Assert.False(cfg.Auto_RewriteRemoveFragment);
            Assert.True(cfg.Cache_Enable);
            Assert.Null(cfg.Cache_Lifetime);
            Assert.Equal(delay, cfg.DownloadDelay);
            Assert.False(cfg.Cookies_Enable);
            Assert.False(cfg.Paused);
            Assert.False(cfg.Paused_Cacher);
            Assert.False(cfg.Paused_Downloader);
            Assert.True(cfg.Auto_AnchorsLinks);
        }

        [Fact]
        public void InitializationParams_Default002()
        {
            int delay = 4321;
            var init = InitializationParams.Default002(delay);

            Assert.IsType<ContentCacher>(init.Cacher);
            Assert.IsType<HttpClientDownloader>(init.Downloader); // Different from 001
            Assert.NotEmpty((init.Downloader as HttpClientDownloader).IncludeRequestHeaders); // Different from 001 | Specific to HttpClientDownloader
            Assert.Empty(init.Parsers);
            Assert.Null(init.StorageEngine);
            Assert.Null(init.SpiderDirectory);

            var cfg = init.ConfigurationPrototype;
            Assert.True(cfg.Auto_RewriteRemoveFragment); // Different from 001
            Assert.True(cfg.Cache_Enable);
            Assert.Null(cfg.Cache_Lifetime);
            Assert.Equal(delay, cfg.DownloadDelay);
            Assert.False(cfg.Cookies_Enable);
            Assert.False(cfg.Paused);
            Assert.False(cfg.Paused_Cacher);
            Assert.False(cfg.Paused_Downloader);
            Assert.True(cfg.Auto_AnchorsLinks);
        }
    }
}
