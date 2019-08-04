using IndexerLib;
using IndexerLib.FilesLookup;
using IndexerLib.LASParser;
using IndexerLucen;
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
            Console.Write("Directory or press Enter to use assembly folder:");
            var directory = Console.ReadLine();

            if (directory == string.Empty)
                directory = Environment.CurrentDirectory;

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

            if (filePaths.Length == 0)
            {
                Console.WriteLine("Have nothing to do, halting");
                Environment.Exit(0);
            }

            Console.WriteLine("Parsing files. Wait a sec...");

            LASFilesParser parser = new LASFilesParser(Environment.ProcessorCount);
            IEnumerable<LASFileData> structuredData = Enumerable.Empty<LASFileData>();
            try
            {
                structuredData = await parser.Parse(filePaths);
            }
            catch (Exception ex)
            {
                LogErrorAndDie(ex.Message);
            }

            Console.WriteLine("Building index. Wait a sec...");

            var indexer = new Indexer();
            Func<LASFileData, IEnumerable<string[]>> fieldsSelector =
                x => x.Sections.SelectMany(s => s.Lines.Select(l => new[] { l.Description, l.Mnemonic }));
            var d1 = structuredData.SelectMany(x => fieldsSelector(x).SelectMany(d => d.Select(z => (x.FilePath, z))));

            try
            {
                indexer.Build(d1);
            }
            catch (Exception ex)
            {
                LogErrorAndDie(ex.Message);
            }

            Console.WriteLine("Index built successfully.");
            Console.WriteLine("Just type field names and hit enter, and I will show results.");
            Console.WriteLine("For example, type 'LTYP' or 'LOG TYPE'");

            while (true)
            {
                Console.WriteLine("---------------");
                var term = Console.ReadLine();

                if (IsExit(term))
                    Environment.Exit(0);

                var res = indexer.Query(term);

                foreach (var item in res)
                {
                    Console.WriteLine($"{item.field} - {item.filePath}");
                }
            }
        }

        private static bool IsExit(string term)
        {
            return string.Compare(term, "q", ignoreCase: true) == 0 ||
                   string.Compare(term, "quit", ignoreCase: true) == 0 ||
                   string.Compare(term, "exit", ignoreCase: true) == 0;

        }

        static void LogErrorAndDie(string errorMsg)
        {
            Console.WriteLine(errorMsg);
            Environment.Exit(1);
        }
    }
}
