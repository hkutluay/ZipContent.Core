using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{
    [TestClass]
    public class GivenValidZipFile
    {
        private readonly IZipContentLister _lister;
      
        public GivenValidZipFile()
        {
            _lister = new ZipContentLister();
        }

        [TestMethod]
        public async Task ExtractedFileCountShouldMatch()
        {
            var partialReader = new PartialFileReader("ZipFiles", "foo.zip");
            var content = await _lister.GetContents(partialReader);
            Assert.AreEqual(content.Count, 1);        
        }

        [TestMethod]
        public async Task ShouldHaveExpectedFileName()
        {

            var partialReader = new PartialFileReader("ZipFiles", "foo.zip");
            var content = await _lister.GetContents(partialReader);           
            
            Assert.AreEqual(content[0].FullName, "foo.txt");
        }
    }
}