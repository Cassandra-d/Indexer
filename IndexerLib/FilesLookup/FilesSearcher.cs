using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IndexerLib.FilesLookup
{

    public class FilesSearcher
    {
        public async Task<IEnumerable<string>> GetFilePaths(string[] folderPaths, FileType ft, CancellationToken ct = default(CancellationToken))
        {
            if (folderPaths.Length == 0)
                throw new ArgumentException("At least one folder path should be provided");
            if (ft == FileType.Unknown)
                throw new ArgumentException("Specify file type");

            var res = new List<string>();
            var mask = FileMaskFactory.Create(ft);
            var tasks = folderPaths.Distinct().Select(f => GetPathForFolder(f, mask, ct)).ToArray();

            try
            {
                await Task.WhenAll(tasks);
            }
            catch { }

            foreach (var t in tasks)
            {
                if (!t.IsFaulted)
                {
                    res.AddRange(t.Result);
                }
            }

            return res.Distinct(); // somebody provided folder and its subfolder
        }

        private async Task<string[]> GetPathForFolder(string folderPath, string fileMask, CancellationToken ct)
        {
            var t = Task.Run(() => { return Directory.EnumerateFiles(folderPath, fileMask, SearchOption.AllDirectories); }, ct);
            await t;
            return t.Result.ToArray();
        }
    }
}
