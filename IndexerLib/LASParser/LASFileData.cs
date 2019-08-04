using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IndexerLib
{
    public class LASFileData
    {
        public bool ContainsData { get; private set; }
        public string FilePath { get; private set; }
        public LASSection[] Sections { get; private set; }

        public LASFileData(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            ContainsData = false;
            FilePath = filePath;
            Sections = Array.Empty<LASSection>();
        }

        public bool Parse(out string error)
        {
            if (!File.Exists(FilePath))
            {
                error = $"File {FilePath} does not exist";
                return false;
            }

            List<LASSection> res = new List<LASSection>();

            try
            {
                using (var sr = new StreamReader(FilePath))
                {
                    foreach (var section in RetrieveSectionsLines(sr))
                    {
                        var firstLine = section.First();
                        var otherLines = section.Skip(1);
                        var sectionName = GetSectionName(firstLine);
                        res.Add(new LASSection(sectionName, otherLines));
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            error = string.Empty;
            Sections = res.ToArray();
            ContainsData = true;
            return true;
        }

        private string GetSectionName(string sectionLine)
        {
            if (string.IsNullOrEmpty(sectionLine))
                throw new ArgumentNullException(nameof(sectionLine));

            // first symbol is tild
            // second symbol is section short name
            return sectionLine[1].ToString();
        }

        private IEnumerable<List<string>> RetrieveSectionsLines(StreamReader sr)
        {
            var buffer = new List<string>();
            var line = string.Empty;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line.StartsWith("~") && buffer.Any())
                {
                    yield return buffer;
                    buffer = new List<string>();
                }
                buffer.Add(line);
            }

            if (buffer.Any())
                yield return buffer;
        }
    }
}
