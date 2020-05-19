using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{
    [TestClass]
    public class GivenValid64BitZipFileWithComments
    {
        private readonly IZipContentLister _lister;

        public GivenValid64BitZipFileWithComments()
        {
            _lister = new ZipContentLister();
        }

        [TestMethod]
        public async Task ExtractedFilesCountShouldMatch()
        {
            var partialReader = new PartialFileReader("ZipFiles", "foo64.zip");
            var content = await _lister.GetContents(partialReader);
            Assert.AreEqual(content.Count, 1);
        }

        [TestMethod]
        public async Task ShouldHaveExpectedFileName()
        {
            var partialReader = new PartialFileReader("ZipFiles", "foo64.zip");
            var content = await _lister.GetContents(partialReader);

            Assert.AreEqual(content[0].FullName, "Documents/foo.txt");
        }
    }
}