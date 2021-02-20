using System;
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

        [Fact]
        public void FastCsv_FeatureSupport_WikipediaExample1()
        {
            string toTest =
@"Year,Make,Model,Extra
1997,Ford,E350,""2,35""
""1997"",""Ford"",""E350"",""quoted""
1997,Ford,E350,""Super, luxurious truck""
1997,Ford,E350,""Super, """"luxurious"""" truck""
1997,Ford,E350,"" Super, luxurious truck ""
2000,Mercury,Cougar,""2,38""
";

            string[][] result = new string[][] {
                new string[]{ "Year", "Make" , "Model", "Extra" },
                new string[]{ "1997", "Ford", "E350", "2,35" },
                new string[]{ "1997", "Ford", "E350", "quoted" },
                new string[]{ "1997", "Ford", "E350", "Super, luxurious truck" },
                new string[]{ "1997", "Ford", "E350", "Super, \"luxurious\" truck" },
                new string[]{ "1997", "Ford", "E350", " Super, luxurious truck " }, // space
                new string[]{ "2000", "Mercury", "Cougar", "2,38" },
            };

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(toTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr, ',').ToArray();

            Assert.Equal(result, lines);
        }
        [Fact]
        public void FastCsv_FeatureSupport_WikipediaExample2()
        {
            string toTest =
@"Year,Make,Model,Description,Price
1997,Ford,E350,""ac, abs, moon"",3000.00
1999,Chevy,""Venture """"Extended Edition"""""","""",4900.00
1999,Chevy,""Venture """"Extended Edition, Very Large"""""",,5000.00
1996,Jeep,Grand Cherokee,""MUST SELL!
air, moon roof, loaded"",4799.00
";

            string[][] result = new string[][] {
                new string[]{ "Year", "Make" , "Model", "Description", "Price" },
                new string[]{ "1997", "Ford", "E350", "ac, abs, moon", "3000.00" },
                new string[]{ "1999", "Chevy", "Venture \"Extended Edition\"", "", "4900.00" },
                new string[]{ "1999", "Chevy", "Venture \"Extended Edition, Very Large\"", "", "5000.00" },
                new string[]{ "1996", "Jeep", "Grand Cherokee", "MUST SELL!\r\nair, moon roof, loaded", "4799.00" },
            };

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(toTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr, ',').ToArray();

            Assert.Equal(result, lines);
        }

        [Fact]
        public void FastCsv_Garbage()
        {
            string toTest = "NO_CAND\";\"DS_CARGO\";\"CD_CARGO\";\"NR_CAND";
            string[] result = { "NO_CAND", "DS_CARGO", "CD_CARGO", "NR_CAND" };

            using var str = new MemoryStream(Encoding.ASCII.GetBytes(toTest));
            using var sr = new StreamReader(str);
            var lines = FastCsv.ReadDelimiter(sr).ToArray();

            Assert.Equal(result, lines[0]);
        }
    }
}
