using System.Linq;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.WrappersTests.HtmlTests
{
    public class MissingTagsTests
    {
        [Fact]
        public void Wrappers_HtmlTag_TagListIsComplete()
        {
            // Check if all tags are specified

            // Get all mapped tags types
            var lstTags = Tag.MappingTable
                .Select(o => o.Item2)
                .OrderBy(o => o.Name)
                .ToArray();
            // Get all `Tag` implementations on assembly
            var types = typeof(Tag).Assembly
                .GetTypes()
                .Where(t => !t.IsInterface)     // only te main classes
                .Where(t => t != typeof(Tag))   // is not Tag itself
                .Where(t => typeof(ITag).IsAssignableFrom(t))
                .OrderBy(o => o.Name)
                .ToArray();

            // Check if booth collections are equal
            Assert.Equal(types, lstTags);
        }
    }
}
