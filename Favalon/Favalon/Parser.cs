using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Favalon
{
    public static class Parser
    {
        public static IEnumerable<Term> Parse(string line) =>
            Parse(new StringReader(line));

        private static Term Recognize(string word)
        {
            if (double.TryParse(word, out var _))
            {
                return new Number(word);
            }
            else if (word.StartsWith("\"") && word.EndsWith("\""))
            {
                return new String(word.Substring(1, word.Length - 2));
            }
            else
            {
                return new Variable(word);
            }
        }

        public static IEnumerable<Term> Parse(TextReader tr, int bufferSize = 4096)
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
