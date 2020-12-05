using System.Threading;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IPartialFileReader
    {
        Task<long> ContentLength();
        Task<byte[]> GetBytes(ByteRange range);

        Task<long> ContentLength(CancellationToken cancellation);

        Task<byte[]> GetBytes(ByteRange range, CancellationToken cancellation);
    }
}
