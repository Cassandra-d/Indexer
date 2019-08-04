using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexerLib
{
    public class LASSection
    {
        public string Name { get; private set; }
        public LASSectionLine[] Lines { get; private set; }

        public LASSection(string name, IEnumerable<string> rawLines)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Lines = rawLines
                .Where(x => !x.StartsWith("#")) // don't want comments
                .Select(x => new LASSectionLine(x)).ToArray();
        }
    }
}
