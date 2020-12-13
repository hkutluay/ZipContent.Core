using System;
using System.Linq;
using ZipContent.Core.Extensions;

namespace ZipContent.Core.Structures
{
    public class EndOfCentralDirectory
    {
        private readonly byte[] headerPattern = new byte[] { 80, 75, 5, 6 };
        public long Size { get; init; }
        public long Start { get; init; }
        public int CommentLength { get; init; }
        public byte[] Bytes { get; init; }


        public EndOfCentralDirectory(byte[] endingBytes)
        {

            int pos = endingBytes.Search(headerPattern);

            if (pos == -1)
                throw new PatternNotFoundException();

            var eocdHeaderBytes = endingBytes.Skip(pos).Take(22).ToArray();

            Size = BitConverter.ToUInt32(eocdHeaderBytes.Skip(12).Take(4).ToArray(), 0);
            Start = BitConverter.ToUInt32(eocdHeaderBytes.Skip(16).Take(4).ToArray(), 0);
            CommentLength = BitConverter.ToUInt16(eocdHeaderBytes.Skip(20).Take(2).ToArray(), 0);
            Bytes = endingBytes.Skip(pos).Take(22 + (CommentLength > 0 ? CommentLength : 0)).ToArray();

            //reset offset of start central directory
            for (int i = 0; i < 4; i++)
                Bytes[i + 16] = 0;

        }

    }
}