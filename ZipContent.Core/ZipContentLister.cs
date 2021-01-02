using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZipContent.Core.Structures;
using ZipContent.Core.Extensions;

namespace ZipContent.Core
{
    public class ZipContentLister : IZipContentLister
    {

        //https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT
        private static readonly byte[] localFileHeaderPattern = new byte[] { 80, 75, 3, 4 };
        private const int endingSize = 5012;
        private readonly IPartialFileReader _partialReader;
        
        public ZipContentLister(IPartialFileReader partialReader)
        {
            _partialReader = partialReader;
        }

        public async Task<bool> IsZipFile(CancellationToken cancellationToken = default)
        {
            var headerBytes = await _partialReader.GetBytes(new ByteRange(0, 4), cancellationToken);
            int headerPos = headerBytes.Search(localFileHeaderPattern);

            return (headerPos != -1);
        }

        public async Task<IList<ZipEntry>> GetContents(CancellationToken cancellationToken = default)
        {
            
            if(!await IsZipFile(cancellationToken))
                throw new FileIsNotaZipException();

            var length = await _partialReader.ContentLength(cancellationToken);

            var readLength = length > endingSize ? endingSize : length;

            var endingBytes = await _partialReader.GetBytes(new ByteRange(length - readLength, length), cancellationToken);

            var zipStructure = new  ZipStructure(endingBytes);

            var centralDirectoryData = await _partialReader.GetBytes(new ByteRange(zipStructure.CentralDirectoryStartPosition, zipStructure.CentralDirectoryEndPosition), cancellationToken);

            if (zipStructure.Zip64EndOfCentralDirectory!= null)
                centralDirectoryData = centralDirectoryData.Concat(zipStructure.Zip64EndOfCentralDirectory.Bytes).ToArray();

            if (zipStructure.Zip64EndOfCentralDirectoryLocator!= null)
                centralDirectoryData = centralDirectoryData.Concat(zipStructure.Zip64EndOfCentralDirectoryLocator.GetBytes(zipStructure.CentralDirectorySize)).ToArray();

            var zipFileFromCentralDirectory = centralDirectoryData.Concat(zipStructure.EndOfCentralDirectory.Bytes).ToArray();

            using Stream stream = new MemoryStream(zipFileFromCentralDirectory);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return archive.Entries.Select(x => new ZipEntry() { FullName = x.FullName, LastWriteTime = x.LastWriteTime, Name = x.Name }).ToList();

        }

    }
}
