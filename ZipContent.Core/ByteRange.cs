namespace ZipContent.Core
{
    public class ByteRange
    {
        public ByteRange()
        {

        }

        public ByteRange(long start, long end)
        {
            Start = start;
            End = end;
        }
        public long Start { get; set; }
        public long End { get; set; }
    }
}
