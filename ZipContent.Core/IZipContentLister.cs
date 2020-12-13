using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IZipContentLister
    {
         Task<bool> IsZipFile(CancellationToken cancellationToken = default);
         Task<IList<ZipEntry>> GetContents(CancellationToken cancellationToken = default);
    }
}
