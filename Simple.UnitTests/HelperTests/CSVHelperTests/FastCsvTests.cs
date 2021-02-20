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
            string toTest =
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

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(toTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr, ',').ToArray();

            Assert.Equal(result, lines);

        }

        [Fact]
        public void FastCsv_SemicolonWithComma()
        {
            string toTest = 
@"Year;Make;Model;Length
1997;Ford;E350;2,35
2000;Mercury;Cougar;2,38
";

            string[][] result = new string[][] {
                new string[]{ "Year", "Make" , "Model", "Length" },
                new string[]{ "1997", "Ford", "E350", "2,35" },
                new string[]{ "2000", "Mercury", "Cougar", "2,38" },
            };

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(toTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr).ToArray();

            Assert.Equal(result, lines);
        }
    }
}
