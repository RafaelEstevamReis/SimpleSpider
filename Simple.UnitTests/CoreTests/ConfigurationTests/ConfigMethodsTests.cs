using System;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.ConfigurationTests
{
    public class ConfigMethodsTests
    {
        Configuration newConfig() => new Configuration();

        // Scheduler
        [Fact]
        public void ConfigMethods_Auto_RewriteRemoveFragment_Tests()
        {
            var cfg = newConfig();
            Assert.False(cfg.Auto_RewriteRemoveFragment);

            cfg.Enable_AutoRewriteRemoveFragment();
            Assert.True(cfg.Auto_RewriteRemoveFragment);

            cfg.Disable_AutoRewriteRemoveFragment();
            Assert.False(cfg.Auto_RewriteRemoveFragment);
        }

        // Cacher

        [Fact]
        public void ConfigMethods_Cache_Enable_Tests()
        {
            var cfg = newConfig();
            Assert.True(cfg.Cache_Enable);

            cfg.Disable_Caching();
            Assert.False(cfg.Cache_Enable);

            cfg.Enable_Caching();
            Assert.True(cfg.Cache_Enable);
        }

        [Fact]
        public void ConfigMethods_Cache_Lifetime_Tests()
        {
            var cfg = newConfig();
            Assert.Null(cfg.Cache_Lifetime);

            cfg.Set_CachingTTL(new TimeSpan(0, 1, 2));
            Assert.Equal(62, cfg.Cache_Lifetime.Value.TotalSeconds);

            cfg.Set_CachingNoLimit();
            Assert.Null(cfg.Cache_Lifetime);
        }

        // Cacher

        [Fact]
        public void ConfigMethods_DownloadDelay_Tests()
        {
            var cfg = newConfig(); // Default is 5s
            Assert.Equal(5000, cfg.DownloadDelay);

            cfg.Set_DownloadDelay(new TimeSpan(0, 1, 2));
            Assert.Equal(62000, cfg.DownloadDelay);

            cfg.Set_DownloadDelay(2000);
            Assert.Equal(2000, cfg.DownloadDelay);
        }
        [Fact]
        public void ConfigMethods_Cookies_Enable_Tests()
        {
            var cfg = newConfig();
            Assert.False(cfg.Cookies_Enable);

            cfg.Enable_Cookies();
            Assert.True(cfg.Cookies_Enable);

            cfg.Disable_Cookies();
            Assert.False(cfg.Cookies_Enable);
        }

        [Fact]
        public void ConfigMethods_DefaultPause_Tests()
        {
            var cfg = newConfig();
            Assert.False(cfg.Paused);
            Assert.False(cfg.Paused_Cacher);
            Assert.False(cfg.Paused_Downloader);
        }


        // Pós-processing

        [Fact]
        public void ConfigMethods_Auto_AnchorsLinks_Tests()
        {
            var cfg = newConfig();
            Assert.True(cfg.Auto_AnchorsLinks);

            cfg.Disable_AutoAnchorsLinks();
            Assert.False(cfg.Auto_AnchorsLinks);

            cfg.Enable_AutoAnchorsLinks();
            Assert.True(cfg.Auto_AnchorsLinks);
        }


    }
}