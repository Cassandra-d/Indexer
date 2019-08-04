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
            var p = new IndexerLib.LASFileData(@"P:\\7125-4-1__WLC_COMPOSITE__1.LAS");

            p.Parse(out var es);
            
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
