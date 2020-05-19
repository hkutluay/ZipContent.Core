using System.Threading.Tasks;

namespace ZipContent.Core
{
    public interface IPartialFileReader
    {
         Task<long> ContentLength();
         Task<byte[]> GetBytes(ByteRange range);
    }

}
