using IndexerLib;
using IndexerLib.FilesLookup;
using IndexerLib.LASParser;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace IndexerCLI
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Directory:");
            var directory = Console.ReadLine();
            var validator = new DirectoryPathValidator();
            var error = validator.Validate(directory);

            if (!string.IsNullOrEmpty(error))
            {
                LogErrorAndDie(error);
            }

            Console.WriteLine("Lookin up for files. Wait a sec...");
            var searcher = new FilesSearcher();
            string[] filePaths = Array.Empty<string>();

            try
            {
                filePaths = await searcher.GetFilePaths(new[] { directory }, FileType.LAS).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Found errors");
                LogErrorAndDie(ex.Message);
            }

            Console.WriteLine($"Found {filePaths.Length} files");
            Console.WriteLine("Parsing files. Wait a sec...");

            LASFilesParser parser = new LASFilesParser(Environment.ProcessorCount);
            IEnumerable<LASFileData> structuredData = Enumerable.Empty<LASFileData>();
            try
            {
                structuredData =  await parser.Parse(filePaths);
            }
            catch (Exception ex)
            {
                LogErrorAndDie(ex.Message);
            }

            Console.WriteLine("Building index. Wait a sec...");



            Console.ReadKey();
        }
        static void LogErrorAndDie(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
    }
}
