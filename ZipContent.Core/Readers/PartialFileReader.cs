using System;
using System.IO;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public class PartialFileReader : IPartialFileReader
    {
        private readonly FileInfo _fileInfo;

        public PartialFileReader(string folder, string fileName)
        {
            _fileInfo = new FileInfo($"{folder}/{fileName}");
        }

        public async Task<long> ContentLength()
        {
            using (var docStream = _fileInfo.OpenRead())
                return docStream.Length;
        }

        public async Task<byte[]> GetBytes(ByteRange range)
        {
            var stream = _fileInfo.OpenRead();
            var br = new BinaryReader(stream);

            byte[] dataArray = new byte[Convert.ToInt32(range.End - range.Start)];
            stream.Seek(Convert.ToInt32(range.Start), SeekOrigin.Begin);

            return br.ReadBytes(Convert.ToInt32(range.End - range.Start));
        }
    }
}
