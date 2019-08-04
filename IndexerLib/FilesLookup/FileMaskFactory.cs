namespace IndexerLib.FilesLookup
{
    internal class FileMaskFactory
    {
        private FileMaskFactory() { }

        public static string Create(FileType ft)
        {
            switch (ft)
            {
                case FileType.Unknown:
                    return string.Empty;
                case FileType.LAS:
                    return "*.las";
                default:
                    return string.Empty;
            }
        }
    }
}
