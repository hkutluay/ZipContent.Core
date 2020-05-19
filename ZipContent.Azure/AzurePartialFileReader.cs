using Azure;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using ZipContent.Core;

namespace ZipContent.Azure
{
    public class AzurePartialFileReader : IPartialFileReader
    {
        private readonly BlobClient _blobClient;

        public AzurePartialFileReader(BlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<long> ContentLength()
        {
            var props = await _blobClient.GetPropertiesAsync();
            return props.Value.ContentLength;
        }

        public async Task<byte[]> GetBytes(ByteRange range)
        {

            var data = await _blobClient.DownloadAsync(new HttpRange(range.Start, range.End - range.Start));
            return StreamToArray(data.Value.Content);

        }

        private byte[] StreamToArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
