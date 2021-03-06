﻿using RafaelEstevam.Simple.Spider.Helper;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.HelperTests.HashTests
{
    public class Crc32Tests
    {
        [Fact]
        public void Helpers_HashCrc32_KnownTrust()
        {
            string r19 = "19111111110302222222200027200000000000000000000000" +
                         "00000000000000000000000000000000000000000000000000" +
                         "00000000000000000000000000000000000000000000000000" +
                         "00000000000000015000000000000000000000000000000000" +
                         "00000000000000000000000000000000000000000000000000" +
                         "00000000000000000000000000000000000000000000000000" +
                         "0000000000000000000000000000000510002488175154";
            Assert.Equal("34e653dd", Crc32.CalcCRC32Hex(r19));
        }
        [Fact]
        public void Helpers_HashCrc32_Url()
        {
            string url = "https://github.com/RafaelEstevamReis/SimpleSpider";
            Assert.Equal("0cff8d25", Crc32.CalcCRC32Hex(url));
        }

        [Fact]
        public void Helpers_HashCrc32_UrlThisTest()
        {
            string url = "https://github.com/RafaelEstevamReis/SimpleSpider/blob/3f4917f321e980e8a58fe889afb569b4c830a4af/Simple.UnitTests/HelperTests/HashTests/Crc32Tests.cs#L30";
            Assert.Equal("2ebe6ed1", Crc32.CalcCRC32Hex(url));
        }
    }
}
