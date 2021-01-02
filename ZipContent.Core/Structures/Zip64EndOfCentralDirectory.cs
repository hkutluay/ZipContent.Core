using System;
using System.Linq;
using ZipContent.Core.Extensions;

namespace ZipContent.Core.Structures
{
    public class Zip64EndOfCentralDirectory
    {
        private readonly byte[] headerPattern = new byte[] { 80, 75, 6, 6 };
        public long Size { get; init; }
        public long Start { get; init; }
        public byte[] Bytes { get; init; }

        public Zip64EndOfCentralDirectory(byte[] endingBytes)
        {

            int pos = endingBytes.Search(headerPattern);

            if (pos == -1)
                throw new PatternNotFoundException();

            Bytes = endingBytes.Skip(pos).Take(56).ToArray();

            Size = BitConverter.ToInt64(Bytes.Skip(40).Take(8).ToArray(), 0);
            Start = BitConverter.ToInt64(Bytes.Skip(48).Take(8).ToArray(), 0);

            for (int i = 0; i < 8; i++)
                Bytes[i + 48] = 0;

        }

    }
}