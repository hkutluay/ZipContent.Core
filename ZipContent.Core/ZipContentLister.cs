using System;
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
        static readonly byte[] localFileHeader = new byte[] { 80, 75, 3, 4 };
        private readonly IPartialFileReader _partialReader;
        
        public ZipContentLister(IPartialFileReader partialReader)
        {
            _partialReader = partialReader;
        }

        public async Task<bool> IsZipFile(CancellationToken cancellationToken = default)
        {
            var headerBytes = await _partialReader.GetBytes(new ByteRange(0, 4), cancellationToken);
            int headerPos = headerBytes.Search(localFileHeader);

            return (headerPos != -1);
        }


        public async Task<IList<ZipEntry>> GetContents(CancellationToken cancellationToken = default)
        {
            
            if(!await IsZipFile(cancellationToken))
                throw new FileIsNotaZipException();

            var length = await _partialReader.ContentLength(cancellationToken);

            var readLength = length > 5012 ? 5012 : length;

            var endingBytes = await _partialReader.GetBytes(new ByteRange(length - readLength, length), cancellationToken);

            var eocd = new EndOfCentralDirectory(endingBytes);
            
            Zip64EndOfCentralDirectory zip64Eocd = null;
            Zip64EndOfCentralDirectoryLocator zip64EocdLocator = null;

            try
            {
                zip64Eocd = new Zip64EndOfCentralDirectory(endingBytes);
                zip64EocdLocator = new Zip64EndOfCentralDirectoryLocator(endingBytes);
            }
            catch
            {

            }

            var start = zip64Eocd?.Start ?? eocd.Start;
            var size = zip64Eocd?.Size ?? eocd.Size;

            var centralDirectoryData = await _partialReader.GetBytes(new ByteRange(start, start + size), cancellationToken);

            if (zip64Eocd != null)
                centralDirectoryData = centralDirectoryData.Concat(zip64Eocd.Bytes).ToArray();

            if (zip64EocdLocator != null)
                centralDirectoryData = centralDirectoryData.Concat(zip64EocdLocator.GetBytes(size)).ToArray();

            var newFile = centralDirectoryData.Concat(eocd.Bytes).ToArray();

            using (Stream stream = new MemoryStream(newFile))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                return archive.Entries.Select(x => new ZipEntry() { FullName = x.FullName, LastWriteTime = x.LastWriteTime, Name = x.Name }).ToList();

        }

    }
}
