using System.IO;

namespace laba_2
{
    class CopyInfo
    {
        public string DirectoryName { get; set; }
        public string Filename { get; set; }
        public string SourceFilename { get; set; }
        public string SourceDirectoryName { get; set; }

        public string Destination
        {
            get
            {
                return Path.Combine(DirectoryName, Filename);
            }
        }
        public string Source
        {
            get
            {
                return Path.Combine(SourceDirectoryName, SourceFilename);
            }
        }
    }
}
