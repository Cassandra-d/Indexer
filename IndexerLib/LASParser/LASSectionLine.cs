using System;
using System.Text;

namespace IndexerLib
{
    public class LASSectionLine
    {
        public string Units { get; private set; }
        public string Mnemonic { get; private set; }
        public string Data { get; private set; }
        public string Description { get; private set; }

        public LASSectionLine(string rawLine)
        {
            if (string.IsNullOrEmpty(rawLine))
                throw new ArgumentNullException(nameof(rawLine));

            Units = string.Empty;
            Mnemonic = string.Empty;
            Data = string.Empty;
            Description = string.Empty;

            var sb = new StringBuilder();
            int currentIndex = 0;

            while (currentIndex < rawLine.Length && rawLine[currentIndex] != '.')
            {
                sb.Append(rawLine[currentIndex]);
                currentIndex += 1;
            }
            Mnemonic = sb.ToString().Trim();
            sb.Clear();
            currentIndex += 1;

            if (rawLine[currentIndex] != ' ')
            {
                while(rawLine[currentIndex] != ' ')
                {
                    sb.Append(rawLine[currentIndex]);
                    currentIndex += 1;
                }
                Units = sb.ToString().Trim();
                sb.Clear();
            }

            int lastColumnIndex = rawLine.Length - 1;
            for (; lastColumnIndex > currentIndex; lastColumnIndex -= 1)
                if (rawLine[lastColumnIndex] == ':')
                    break;

            Data = rawLine.Substring(currentIndex,  lastColumnIndex - currentIndex).Trim();
            Description = rawLine.Substring(lastColumnIndex + 1).Trim();
        }
    }
}
