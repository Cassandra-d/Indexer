using IndexerLib;
using IndexerLib.FilesLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var fs = new FilesSearcher();
            var files = fs.GetFilePaths(new[] { @"P:\shitty files", @"P:\shitty files\2" }, FileType.LAS).ConfigureAwait(false).GetAwaiter().GetResult();
            var r = files.Select(f =>
            {
                var parser = new LASFileData(f);
                var res = parser.Parse(out var er);
                if (res)
                    return parser.Sections;
                return null;
            }
                ).Where(x => x != null)
                .ToArray();


            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
