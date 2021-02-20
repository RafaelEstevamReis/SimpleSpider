using System.IO;
using System.Linq;
using System.Text;
using RafaelEstevam.Simple.Spider.Helper;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.HelperTests.CSVHelperTests
{
    public class FastCsvTests
    {
        [Fact]
        public void FastCsv_BaseTest()
        {
            string originalToTest =
@"Year,Make,Model
1997,Ford,E350
2000,Mercury,Cougar
1997,Ford,""E350""";

            string[][] result = new string[][] {
                new string[]{ "Year", "Make" , "Model" },
                new string[]{ "1997", "Ford", "E350" },
                new string[]{ "2000", "Mercury", "Cougar" },
                new string[]{ "1997", "Ford", "E350" },
            };

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(originalToTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr, ',').ToArray();

            Assert.Equal(result, lines);

        }
    }
}
