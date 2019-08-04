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
        public List<LASSection> Sections { get; private set; }

        public LASFileData(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            ContainsData = false;
            FilePath = filePath;
            Sections = new List<LASSection>();
        }

        public bool Parse(out List<string> errors)
        {
            errors = new List<string>();

            if (!File.Exists(FilePath))
            {
                errors.Add($"File {FilePath} does not exist");
                return false;
            }

            try
            {
                using (var sr = new StreamReader(FilePath))
                {
                    foreach (var section in RetrieveSectionsLines(sr))
                    {
                        var firstLine = section.First();
                        var otherLines = section.Skip(1);
                        var sectionName = GetSectionName(firstLine);
                        Sections.Add(new LASSection(sectionName, otherLines));
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }

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
