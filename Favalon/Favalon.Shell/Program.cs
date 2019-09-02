using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Favalon
{
    class Program
    {
        static void Main(string[] args)
        {
            var tr = Console.In;
            var parser = new Parser();

            while (true)
            {
                var line = tr.ReadLine();
                if (line == null)
                {
                    break;
                }

                foreach (var token in parser.Tokenize(line, false))
                {
                    Console.WriteLine(token);
                }
            }
        }
    }
}
