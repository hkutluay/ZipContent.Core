using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{
    [TestClass]
    public class GivenInvalidZipFile
    {
       
        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThrowExceptionForEmptyZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "zero-file.zip");
            var lister = new ZipContentLister(partialReader);
            await lister.GetContents();
        }


        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThrowExceptionForNonZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "not-a-zip.zip");
            var lister = new ZipContentLister(partialReader);
            await lister.GetContents();
        }

        [TestMethod]
        [ExpectedException(typeof(FileIsNotaZipException))]
        public async Task ShouldThroExceptionForZeroByteZipFile()
        {
            var partialReader = new PartialFileReader("ZipFiles", "zero-byte.zip");
            var lister = new ZipContentLister(partialReader);
            await lister.GetContents();
        }
    }

}