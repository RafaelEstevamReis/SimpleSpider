using RafaelEstevam.Simple.Spider.Helper;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.HelperTests.CSVHelperTests
{
    public class SplitLineTests
    {
        [Theory]
        // Empty
        [InlineData(new string[] { "" }, "", ';')]
        [InlineData(new string[] { "" }, "", ',')]
        [InlineData(new string[] { "", "" }, ";", ';')]
        [InlineData(new string[] { "", "" }, ",", ',')]
        [InlineData(new string[] { ";", ";" }, ";,;", ',')]
        public void CSVHelpers_SplitLineTests_Base(string[] result, string line, char delimiter)
        {
            Assert.Equal(result, CSVHelper.splitLine(line, delimiter));
        }

        [Theory]
        [InlineData(new string[] { "aaa", "bbb", "ccc" }, "\"aaa\",\"bbb\",\"ccc\"")]
        [InlineData(new string[] { "a,a;a", "b\tbb" }, "\"a,a;a\",\"b\tbb\"")]
        [InlineData(new string[] { "zzz", "yyy", "xxx" }, "zzz,yyy,xxx")]
        public void CSVHelpers_SplitLineTests_RFC4180(string[] result, string line)
        {
            //RFC4180 - multiline is outside of the scope
            Assert.Equal(result, CSVHelper.splitLine(line, ','));
        }
        [Theory]
        [InlineData(new string[] { "a,a;a", "b\tb'b" }, "\"a,a;a\",\"b\tb'b\"")]
        public void CSVHelpers_SplitLineTests_Quoting(string[] result, string line)
        {
            //RFC4180 - multiline is outside of the scope
            Assert.Equal(result, CSVHelper.splitLine(line, ','));
        }
    }
}
