using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerLib.FilesLookup
{
    public enum FileType
    {
        Unknown = 0,
        LAS = 1
    }

    public class FilesSearcher
    {
        public async Task<IEnumerable<string>> GetFilePaths(string[] folderPaths)
        {
            if (folderPaths.Length == 0)
                throw new ArgumentException("At least one folder path should be provided");

            var tasks = new List<Task>()
            foreach (var folder in folderPaths)
            {

            }
        }
    }
}
