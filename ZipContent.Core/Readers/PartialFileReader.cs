using System;
using System.IO;
using System.Threading;
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

        public Task<long> ContentLength()
        {
            var task = new Task<long>(() =>
            {
                using (var docStream = _fileInfo.OpenRead())
                {
                    return docStream.Length;
                }
            });
            task.RunSynchronously(TaskScheduler.Current);
            return task;
        }

        public Task<byte[]> GetBytes(ByteRange range)
        {
            var task = new Task<byte[]>(() =>
            {
                var stream = _fileInfo.OpenRead();
                var br = new BinaryReader(stream);

                byte[] dataArray = new byte[Convert.ToInt32(range.End - range.Start)];
                stream.Seek(Convert.ToInt32(range.Start), SeekOrigin.Begin);

                return br.ReadBytes(Convert.ToInt32(range.End - range.Start));
            });
            task.RunSynchronously(TaskScheduler.Current);
            return task;
        }

        public Task<long> ContentLength(CancellationToken cancellation)
        {
            return ContentLength();
        }

        public Task<byte[]> GetBytes(ByteRange range, CancellationToken cancellation)
        {
            return GetBytes(range);
        }
    }
}
