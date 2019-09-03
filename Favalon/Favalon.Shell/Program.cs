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
            var lexer = new Lexer();

            while (true)
            {
                var line = tr.ReadLine();
                if (line == null)
                {
                    break;
                }

                foreach (var token in lexer.Lex(line, false))
                {
                    Console.WriteLine(token);
                }
            }
        }
    }
}
