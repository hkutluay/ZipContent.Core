using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{

    [TestClass]
    public class GivenNestedZipFile
    {

        private readonly IZipContentLister _lister;
        public GivenNestedZipFile()
        {
            _lister = new ZipContentLister();
        }

        [TestMethod]
        public async Task ExtractedFilesCountShouldMatch()
        {
            var partialReader = new PartialFileReader("ZipFiles", "nested.zip");
             var content = await _lister.GetContents(partialReader);

            Assert.AreEqual(content.Count, 1);
        }
    }
}