using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IZipContent
    {
        Task<IList<ZipEntry>> GetContents(string folder, string fileName);
    }
}
