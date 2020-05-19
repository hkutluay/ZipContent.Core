using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IZipContentLister
    {
         Task<IList<ZipEntry>> GetContents(IPartialFileReader partialReader);
    }
}
