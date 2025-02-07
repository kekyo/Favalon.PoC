﻿using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Favalon
{
    public static class Lexer
    {
        public static IEnumerable<Term> Lex(string line) =>
            Lex(new StringReader(line), (line.Length < 4096) ? line.Length : 4096);

        private static Term Recognize(string word)
        {
            if (double.TryParse(word, out var _))
            {
                return new Number(word);
            }
            else if (word.StartsWith("\"") && word.EndsWith("\""))
            {
                // TODO: unescape
                return new Favalon.Terms.String(word.Substring(1, word.Length - 2));
            }
            else
            {
                return new Variable(word);
            }
        }

        public static IEnumerable<Term> Lex(TextReader tr, int bufferSize = 4096)
        {
            var buffer = new char[bufferSize];
            var inWord = false;
            var sb = new StringBuilder();

            while (true)
            {
                var read = tr.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    break;
                }

                var startPosition = 0;
                for (var index = 0; index < read; index++)
                {
                    var ch = buffer[index];
                    if (char.IsWhiteSpace(ch))
                    {
                        if (inWord)
                        {
                            sb.Append(buffer, startPosition, index - startPosition);

                            yield return Recognize(sb.ToString());

                            inWord = false;
                            sb.Clear();
                        }
                        startPosition = index + 1;
                    }
                    else
                    {
                        inWord = true;
                    }
                }

                if (startPosition < read)
                {
                    sb.Append(buffer, startPosition, read - startPosition);
                }
            }

            if (sb.Length >= 1)
            {
                yield return Recognize(sb.ToString());
            }
        }
    }
}
