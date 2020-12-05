using System.Threading;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IPartialFileReader
    {
        
        Task<long> ContentLength(CancellationToken cancellationToken = default);

        Task<byte[]> GetBytes(ByteRange range, CancellationToken cancellationToken = default);
    }
}
