namespace ZipContent.Core.Structures
{
    public class ZipStructure
    {

        public EndOfCentralDirectory EndOfCentralDirectory { get; init; }
        public Zip64EndOfCentralDirectory Zip64EndOfCentralDirectory { get; init; }
        public Zip64EndOfCentralDirectoryLocator Zip64EndOfCentralDirectoryLocator { get; init; }

        public long CentralDirectoryStartPosition => Zip64EndOfCentralDirectory?.Start ?? EndOfCentralDirectory.Start;
        public long CentralDirectoryEndPosition => CentralDirectoryStartPosition + CentralDirectorySize;
        public long CentralDirectorySize => Zip64EndOfCentralDirectory?.Size ?? EndOfCentralDirectory.Size;
       
        public ZipStructure(byte[] endingBytes)
        {
            EndOfCentralDirectory = new EndOfCentralDirectory(endingBytes);

            try
            {
                Zip64EndOfCentralDirectory = new Zip64EndOfCentralDirectory(endingBytes);
                Zip64EndOfCentralDirectoryLocator = new Zip64EndOfCentralDirectoryLocator(endingBytes);
            }
            catch {}

        }

    }
}