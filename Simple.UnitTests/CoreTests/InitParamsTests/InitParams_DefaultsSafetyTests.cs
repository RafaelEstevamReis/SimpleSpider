using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CoreTests.InitParamsTests
{
    /// <summary>
    /// Uses serialization to make SURE nothing changed. 
    /// Properties will be enumerated automatically, any new property not handled will break here
    /// </summary>
    public class InitParams_DefaultsSafetyTests
    {
        [Fact]
        public void InitializationParams_SafetyCheck_Default000()
        {
            var init = InitializationParams.Default000();
            var ls = serializeParams(init).ToArray();

            string[] expected = {
                "Cacher: RafaelEstevam.Simple.Spider.Cachers.ContentCacher",
                "Downloader: RafaelEstevam.Simple.Spider.Downloaders.WebClientDownloader",
                "SpiderDirectory: ",
                "Parsers: 0",
                "Config.SpiderDirectory ",
                "Config.SpiderDataDirectory ",
                "Config.Spider_LogFile ",
                "Config.Logger ",
                "Config.Auto_RewriteRemoveFragment False",
                "Config.Cache_Enable True",
                "Config.Cache_Lifetime ",
                "Config.DownloadDelay 5000",
                "Config.Cookies_Enable False",
                "Config.Paused False",
                "Config.Paused_Cacher False",
                "Config.Paused_Downloader False",
                "Config.Auto_AnchorsLinks False",
                "Config.SpiderAllowHostViolation False",
            };
            Assert.Equal(expected.Length, ls.Length);

            for (int i = 0; i < ls.Length; i++)
            {
                Assert.Equal(expected[i], ls[i]);
            }
        }
        [Fact]
        public void InitializationParams_SafetyCheck_Default001()
        {
            var init = InitializationParams.Default001();
            var ls = serializeParams(init).ToArray();

            string[] expected = {
                "Cacher: RafaelEstevam.Simple.Spider.Cachers.ContentCacher",
                "Downloader: RafaelEstevam.Simple.Spider.Downloaders.WebClientDownloader",
                "SpiderDirectory: ",
                "Parsers: 0",
                "Config.SpiderDirectory ",
                "Config.SpiderDataDirectory ",
                "Config.Spider_LogFile ",
                "Config.Logger ",
                "Config.Auto_RewriteRemoveFragment False",
                "Config.Cache_Enable True",
                "Config.Cache_Lifetime ",
                "Config.DownloadDelay 5000",
                "Config.Cookies_Enable False",
                "Config.Paused False",
                "Config.Paused_Cacher False",
                "Config.Paused_Downloader False",
                "Config.Auto_AnchorsLinks True",
                "Config.SpiderAllowHostViolation False",
            };
            Assert.Equal(expected.Length, ls.Length);

            for (int i = 0; i < ls.Length; i++)
            {
                Assert.Equal(expected[i], ls[i]);
            }
        }

        [Fact]
        public void InitializationParams_SafetyCheck_Default002()
        {
            var init = InitializationParams.Default002();
            var ls = serializeParams(init).ToArray();

            string[] expected = {
                "Cacher: RafaelEstevam.Simple.Spider.Cachers.ContentCacher",
                "Downloader: RafaelEstevam.Simple.Spider.Downloaders.HttpClientDownloader",
                "SpiderDirectory: ",
                "Parsers: 0",
                "Config.SpiderDirectory ",
                "Config.SpiderDataDirectory ",
                "Config.Spider_LogFile ",
                "Config.Logger ",
                "Config.Auto_RewriteRemoveFragment True",
                "Config.Cache_Enable True",
                "Config.Cache_Lifetime ",
                "Config.DownloadDelay 5000",
                "Config.Cookies_Enable False",
                "Config.Paused False",
                "Config.Paused_Cacher False",
                "Config.Paused_Downloader False",
                "Config.Auto_AnchorsLinks True",
                "Config.SpiderAllowHostViolation False",
            };
            Assert.Equal(expected.Length, ls.Length);

            for (int i = 0; i < ls.Length; i++)
            {
                Assert.Equal(expected[i], ls[i]);
            }
        }

        private static IEnumerable<string> serializeParams(InitializationParams init)
        {
            var cfg = init.ConfigurationPrototype;
            var type = init.GetType();
            foreach (var p in type.GetProperties())
            {
                if (!p.CanRead) continue;
                if (p.Name == "Parsers") continue; // Will be checked later
                if (p.Name == "ConfigurationPrototype") continue; // Will be checked later
                if (p.Name == "StorageEngine") continue; // Will be checked later
                string val = "";
                if (p.PropertyType.IsInterface)
                {
                    val = p.GetValue(init).GetType().FullName;
                }
                else
                {
                    val = p.GetValue(init)?.ToString();
                }
                yield return $"{p.Name}: {val}";
            }

            yield return $"Parsers: {init.Parsers.Count()}";
            for (int i = 0; i < init.Parsers.Count; i++)
            {
                yield return $"Parsers[{i}]: {init.Parsers[i]}";
            }

            type = cfg.GetType();
            foreach (var p in type.GetProperties())
            {
                if (!p.CanRead) continue;

                var pVal = p.GetValue(cfg);
                yield return $"Config.{p.Name} {pVal}";
            }
        }

    }
}
