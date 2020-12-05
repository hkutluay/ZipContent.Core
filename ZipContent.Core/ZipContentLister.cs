using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ZipContent.Core
{
    public class ZipContentLister : IZipContentLister
    {

        //https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT
        static readonly byte[] eocdHeader = new byte[] { 80, 75, 5, 6 };
        static readonly byte[] zip64EocdHeader = new byte[] { 80, 75, 6, 6 };
        static readonly byte[] zip64EocdLocatorHeader = new byte[] { 80, 75, 6, 7 };
        static readonly byte[] localFileHeader = new byte[] { 80, 75, 3, 4 };

        public async Task<IList<ZipEntry>> GetContents(IPartialFileReader partialReader, CancellationToken cancellationToken = default)
        {
            var length = await partialReader.ContentLength();

            var readLength = length > 5012 ? 5012 : length;

            var headerBytes = await partialReader.GetBytes(new ByteRange(0, 4), cancellationToken);
            int headerPos = Search(headerBytes, localFileHeader);

            if (headerPos == -1)
                throw new FileIsNotaZipException();


            var endingBytes = await partialReader.GetBytes(new ByteRange(length - readLength, length), cancellationToken);

            int pos = Search(endingBytes, eocdHeader);

            var eocdHeaderBytes = endingBytes.Skip(pos).Take(22).ToArray();

            long size = BitConverter.ToUInt32(eocdHeaderBytes.Skip(12).Take(4).ToArray(), 0);
            long start = BitConverter.ToUInt32(eocdHeaderBytes.Skip(16).Take(4).ToArray(), 0);
            int commentLength = BitConverter.ToUInt16(eocdHeaderBytes.Skip(20).Take(2).ToArray(), 0);

            eocdHeaderBytes = endingBytes.Skip(pos).Take(22 + (commentLength > 0 ? commentLength : 0)).ToArray();


            byte[] zip64EocdLocatorHeaderBytes = null, zip64EocdHeaderBytes = null;

            int zip64EocdHeaderPos = Search(endingBytes, zip64EocdHeader);
            int zip64EocdLocatorHeaderPos = Search(endingBytes, zip64EocdLocatorHeader);


            if (zip64EocdHeaderPos != -1 && zip64EocdLocatorHeaderPos != -1) //zip64bit
            {
                zip64EocdHeaderBytes = endingBytes.Skip(zip64EocdHeaderPos).Take(56).ToArray();

                size = BitConverter.ToInt64(zip64EocdHeaderBytes.Skip(40).Take(8).ToArray(), 0);
                start = BitConverter.ToInt64(zip64EocdHeaderBytes.Skip(48).Take(8).ToArray(), 0);

                zip64EocdLocatorHeaderBytes = endingBytes.Skip(zip64EocdLocatorHeaderPos).Take(20).ToArray();
            }

            var centralDirectoryData = await partialReader.GetBytes(new ByteRange(start, start + size), cancellationToken);

            for (int i = 0; i < 4; i++)
                eocdHeaderBytes[i + 16] = 0;

            if (zip64EocdLocatorHeaderBytes != null)
            {
                for (int i = 0; i < 8; i++)
                    zip64EocdHeaderBytes[i + 48] = 0;

                byte[] offset = BitConverter.GetBytes(Convert.ToInt64(centralDirectoryData.Length));

                for (int i = 0; i < 8; i++)
                    zip64EocdLocatorHeaderBytes[i + 8] = offset[i];
            }

            if (zip64EocdHeaderBytes != null)
                centralDirectoryData = centralDirectoryData.Concat(zip64EocdHeaderBytes).ToArray();

            if (zip64EocdLocatorHeaderBytes != null)
                centralDirectoryData = centralDirectoryData.Concat(zip64EocdLocatorHeaderBytes).ToArray();

            var newFile = centralDirectoryData.Concat(eocdHeaderBytes).ToArray();

            using (Stream stream = new MemoryStream(newFile))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                return archive.Entries.Select(x => new ZipEntry() { FullName = x.FullName, LastWriteTime = x.LastWriteTime, Name = x.Name }).ToList();

        }
        
        
        private int Search(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;

            for (int i = c; i > -1; i--)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }

            return -1;
        }

    }
}
