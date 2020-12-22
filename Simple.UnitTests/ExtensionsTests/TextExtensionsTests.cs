using RafaelEstevam.Simple.Spider.Extensions;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.ExtensionsTests
{
    public class TextExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("a", "a")]
        [InlineData("a a", "a a")]
        [InlineData("     a a", "a a")]
        [InlineData("a a    ", "a a")]
        [InlineData(" a a ", "a a")]
        [InlineData(" a\na ", "a a")]
        [InlineData(" a\ta ", "a a")]
        [InlineData(" a\ra ", "a a")]
        [InlineData(" a \r\n\t a ", "a a")]
        [InlineData(" a\t\r\n a ", "a a")]
        public void Extensions_TextExtensions_RemoveExcessiveWhitespacesTrue(string Input, string Expected)
        {
            var actual = TextExtensions.RemoveExcessiveWhitespaces(Input, true);
            Assert.Equal(Expected, actual);
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("a", "a")]
        [InlineData("a a", "a a")]
        [InlineData("     a a", "a a")]
        [InlineData("a a    ", "a a")]
        [InlineData(" a a ", "a a")]
        [InlineData(" a\na ", "a\na")]
        [InlineData(" a\ta ", "a\ta")]
        [InlineData(" a\ra ", "a\ra")]
        [InlineData(" a \r\n\t a ", "a a")]
        [InlineData(" a\t\r\n a ", "a\ta")]
        public void Extensions_TextExtensions_RemoveExcessiveWhitespacesFalse(string Input, string Expected)
        {
            var actual = TextExtensions.RemoveExcessiveWhitespaces(Input, false);
            Assert.Equal(Expected, actual);
        }
    }
}

