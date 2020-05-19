using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{
    [TestClass]
    public class GivenInvalidZipFile
    {
        private readonly IZipContentLister _lister;
        public GivenInvalidZipFile()
        {
            _lister = new ZipContentLister();
        }

        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThrowExceptionForEmptyZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "zero-file.zip");
            await _lister.GetContents(partialReader);
        }


        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThrowExceptionForNonZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "not-a-zip.zip");
            await _lister.GetContents(partialReader);
        }

        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThroExceptionForZeroByteZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "zero-byte.zip");
            await _lister.GetContents(partialReader);
        }
    }

}