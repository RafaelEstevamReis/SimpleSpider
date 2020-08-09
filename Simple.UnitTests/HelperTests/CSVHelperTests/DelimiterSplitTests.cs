using Net.RafaelEstevam.Spider.Helper;
using System.Linq;
using Xunit;

namespace Net.RafaelEstevam.Spider.UnitTests.HelperTests.CSVHelperTests
{
    public class DelimiterSplitTests
    {
        [Fact]
        public void CSVHelpers_DelimiterSplitTests_Base()
        {
            string delimiter = "abc;\"def\";ghi\n123;'456';\"789\"";
            var lines = delimiter.Split('\n');

            var items = CSVHelper.DelimiterSplit(lines, ';').ToArray();
            Assert.Equal(2, items.Length);
            Assert.Equal(new string[] { "abc", "def", "ghi" }, items[0]);
            Assert.Equal(new string[] { "123", "'456'", "789" }, items[1]);
        }
        [Fact]
        public void CSVHelpers_DelimiterSplitTests_MSExcelCSV()
        {
            string delimiter = "abc\t\"def\"\tghi\n123\t'456'\t\"789\"";
            var lines = delimiter.Split('\n');

            var items = CSVHelper.DelimiterSplit(lines, '\t').ToArray();
            Assert.Equal(2, items.Length);
            Assert.Equal(new string[] { "abc", "def", "ghi" }, items[0]);
            Assert.Equal(new string[] { "123", "'456'", "789" }, items[1]);
        }
    }
}
