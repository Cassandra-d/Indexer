using System.IO;

namespace IndexerCLI
{
    partial class Program
    {
        public class DirectoryPathValidator
        {
            public DirectoryPathValidator() { }

            public string Validate(string directoryPath)
            {
                if (string.IsNullOrEmpty(directoryPath))
                    return "Directory path canno be empty";
                if (!Directory.Exists(directoryPath))
                    return $"Directory {directoryPath} doesn't exist";
                return string.Empty;
            }
        }
    }
}
