namespace ZipContent.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static int Search(this byte[] src, byte[] pattern)
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