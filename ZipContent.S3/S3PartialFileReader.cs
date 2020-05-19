using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;
using ZipContent.Core;

namespace ZipContent.S3
{
    public class S3PartialFileReader : IPartialFileReader
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;
        private readonly string _key;

        public S3PartialFileReader(IAmazonS3 s3, string bucket, string key)
        {
            _s3 = s3;
            _bucket = bucket;
            _key = key;
        }

        public async Task<long> ContentLength()
        {
            var metadata = await _s3.GetObjectMetadataAsync(_bucket, _key);
            return metadata.ContentLength;
        }

        public async Task<byte[]> GetBytes(Core.ByteRange range)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _bucket,
                Key = _key,
                ByteRange = new Amazon.S3.Model.ByteRange(range.Start, range.End)
            };
            var response = await _s3.GetObjectAsync(request);
            return StreamToArray(response.ResponseStream);
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
