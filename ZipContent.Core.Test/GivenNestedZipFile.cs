using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ZipContent.Core.Test
{

    [TestClass]
    public class GivenNestedZipFile
    {


        [TestMethod]
        public async Task ExtractedFilesCountShouldMatch()
        {
            var partialReader = new PartialFileReader("ZipFiles", "nested.zip");
            var lister = new ZipContentLister(partialReader);
            var content = await lister.GetContents();

            Assert.AreEqual(content.Count, 1);
        }
    }
}