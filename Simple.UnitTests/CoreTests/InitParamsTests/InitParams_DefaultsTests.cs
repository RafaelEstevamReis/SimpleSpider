using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.CoreTests.InitParamsTests
{
    public class InitParams_DefaultsTests
    {
        [Fact]
        public void InitializationParams_Default001()
        {
            int delay = 4321;
            var init = InitializationParams.Default001(delay);

            Assert.IsType<Cachers.ContentCacher>(init.Cacher);
            Assert.IsType<Downloaders.WebClientDownloader>(init.Downloader);
            Assert.Empty(init.Parsers);
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

            Assert.IsType<Cachers.ContentCacher>(init.Cacher);
            Assert.IsType<Downloaders.HttpClientDownloader>(init.Downloader); // Different from 001
            Assert.Empty(init.Parsers);
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
