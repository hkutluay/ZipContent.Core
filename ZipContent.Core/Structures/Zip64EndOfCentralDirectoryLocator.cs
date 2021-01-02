using System;
using System.Linq;
using ZipContent.Core.Extensions;

namespace ZipContent.Core.Structures
{
    public class Zip64EndOfCentralDirectoryLocator
    {
        private readonly byte[] headerPattern = new byte[] { 80, 75, 6, 7 };

        public byte[] Bytes { get; init; }

        public Zip64EndOfCentralDirectoryLocator(byte[] endingBytes)
        {

            int pos = endingBytes.Search(headerPattern);

            if (pos == -1)
                throw new PatternNotFoundException();

            Bytes = endingBytes.Skip(pos).Take(20).ToArray();

        }

        public byte[] GetBytes(long centralDirectoryDataLength)
        {

            byte[] offset = BitConverter.GetBytes(Convert.ToInt64(centralDirectoryDataLength));

            for (int i = 0; i < 8; i++)
                Bytes[i + 8] = offset[i];

            return Bytes;

        }

    }
}