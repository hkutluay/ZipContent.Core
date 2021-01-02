using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{
    [TestClass]
    public class GivenValidZipFile
    {
        [TestMethod]
        public async Task ExtractedFileCountShouldMatch()
        {
            var partialReader = new PartialFileReader("ZipFiles", "foo.zip");
            var lister = new ZipContentLister(partialReader);
            var content = await lister.GetContents();
            Assert.AreEqual(content.Count, 1);        
        }

        [TestMethod]
        public async Task ShouldHaveExpectedFileName()
        {

            var partialReader = new PartialFileReader("ZipFiles", "foo.zip");
            var lister = new ZipContentLister(partialReader);
            var content = await lister.GetContents();        
            
            Assert.AreEqual(content[0].FullName, "foo.txt");
        }
    }
}