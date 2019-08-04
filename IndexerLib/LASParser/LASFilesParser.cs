using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexerLib.LASParser
{
    public class LASFilesParser
    {
        private int threadsCount;

        public LASFilesParser(int degreeOfParallelism = 1)
        {
            threadsCount = degreeOfParallelism;
        }

        public async Task<IEnumerable<LASFileData>> Parse(string[] filePaths)
        {
            if (filePaths == null || filePaths.Length == 0)
                throw new ArgumentException("Must provide files");

            List<LASFileData> res = new List<LASFileData>();

            var elementsPerThread = Math.Max(filePaths.Length / threadsCount, 1);
            var tasks = filePaths
                .Batch(elementsPerThread)
                .Select(b => Task.Run(() => { return ParseInternal(b); }));

            await Task.WhenAll(tasks);

            foreach (var t in tasks)
            {
                if (!t.IsFaulted)
                    res.AddRange(t.Result);
            }

            return res;
        }

        private IEnumerable<LASFileData> ParseInternal(IEnumerable<string> filePaths)
        {
            var res = new List<LASFileData>();
            foreach (var fp in filePaths)
            {
                var d = new LASFileData(fp);
                d.Parse(out var error);
                if (d.ContainsData)
                    res.Add(d);
            }
            return res.AsEnumerable();
        }
    }
}
