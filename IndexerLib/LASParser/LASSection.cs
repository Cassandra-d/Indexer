using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexerLib
{
    public class LASSection
    {
        public string Name { get; private set; }
        public List<LASSectionLine> Lines { get; private set; }

        public LASSection(string name, IEnumerable<string> enumerable)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Lines = enumerable
                .Where(x => !x.StartsWith("#")) // don't want comments
                .Select(x => new LASSectionLine(x)).ToList();
        }
    }
}
