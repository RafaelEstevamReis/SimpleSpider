using System;
using System.IO;
using RafaelEstevam.Simple.Spider.Cachers;
using Xunit;

namespace RafaelEstevam.Simple.Spider.UnitTests.CachersTests.RequestCacherTests
{
    public class GetCacheFileFullNameTests
    {
        [Fact]
        public void Cachers_RequestCacher_FileNameBaseTest1()
        {
            string baseFolder = "A:\\floppy";

            var uri = new Uri("http://github.com/RafaelEstevamReis/SimpleSpider/blob/3f4917f321e980e8a58fe889afb569b4c830a4af/Simple.UnitTests/HelperTests/HashTests/Crc32Tests.cs?a=2&b=3#L30");

            var fName = RequestCacher.getCacheFileFullName(baseFolder, uri);

            // fix tests on linux systems
            fName = fName.Replace("/", "\\");

            Assert.Equal(@"A:\floppy\5d\perTests_HashTests_Crc32Tests.cs_5d51823616dc8e14.tmp", 
                         fName);
        }
        [Fact]
        public void Cachers_RequestCacher_FileNameBaseTest2()
        {
            string baseFolder = "A:\\floppy";

            var uri = new Uri("https://books.toscrape.com/catalogue/category/books/mystery_3/page-2.html");

            var fName = RequestCacher.getCacheFileFullName(baseFolder, uri);

            // fix tests on linux systems
            fName = fName.Replace("/", "\\");

            Assert.Equal(@"A:\floppy\92\gory_books_mystery_3_page-2.html_92b85b4f2d8dcb1f.tmp",
                         fName);
        }
        [Fact]
        public void Cachers_RequestCacher_FileNameBaseTest2_Query()
        {
            string baseFolder = "A:\\floppy";

            var uri = new Uri("https://books.toscrape.com/catalogue/category/books/mystery_3/page-2.html?a=b&c=d#1");

            var fName = RequestCacher.getCacheFileFullName(baseFolder, uri);

            // fix tests on linux systems
            fName = fName.Replace("/", "\\");

            Assert.Equal(@"A:\floppy\92\gory_books_mystery_3_page-2.html_92b85b4f50198596.tmp",
                         fName);
        }

    }
}
