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

        [Fact]
        public void CSVHelpers_SplitLineTests_WrongQuoting()
        {
            string line = "NO_CAND\";\"DS_CARGO\";\"CD_CARGO\";\"NR_CAND";
            string[] result = { "NO_CAND", "DS_CARGO", "CD_CARGO", "NR_CAND" };

            Assert.Equal(result, CSVHelper.splitLine(line, ';'));
        }
        [Fact]
        public void CSVHelpers_SplitLineTests_WrongQuoting_RealCase()
        {
            // This line broke the algorithm while reading public data about brazil's election candidates
            string line = "NO_CAND\";\"DS_CARGO\";\"CD_CARGO\";\"NR_CAND\";\"SG_UE_SUP\";\"NO_UE\";\"SG_UE\";\"NR_PART\";\"SG_PART\";\"VR_DESPESA\";\"DT_DOC_DESP\";\"RTRIM(LTRIM(DR.DS_TITULO))\";\"CD_TITULO\";\"DECODE(REC.TP_RECURSO,0,'EMESPÉCIE',1,'CHEQUE',2,'ESTIMADO','NÃOINFORMADO')\";\"TP_RECURSO\";\"NR_DOC_DESP\";\"DS_TIPO_DOCUMENTO\";\"CD_DOC\";\"NO_FOR\";\"CD_CPF_CGC\";\"DS_MUNIC\";\"RV_MEANING";

            string[] result = { "NO_CAND", "DS_CARGO", "CD_CARGO", "NR_CAND", "SG_UE_SUP",
                                "NO_UE", "SG_UE", "NR_PART", "SG_PART", "VR_DESPESA",
                                "DT_DOC_DESP", "RTRIM(LTRIM(DR.DS_TITULO))", "CD_TITULO",
                                "DECODE(REC.TP_RECURSO,0,'EMESPÉCIE',1,'CHEQUE',2,'ESTIMADO','NÃOINFORMADO')",
                                "TP_RECURSO", "NR_DOC_DESP", "DS_TIPO_DOCUMENTO", "CD_DOC",
                                "NO_FOR", "CD_CPF_CGC", "DS_MUNIC", "RV_MEANING" };


            Assert.Equal(result, CSVHelper.splitLine(line, ';'));
        }
    }
}
