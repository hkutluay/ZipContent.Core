using System;

namespace ZipContent.Core
{
    public class ZipEntry
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTimeOffset LastWriteTime { get; set; }
    }
}
