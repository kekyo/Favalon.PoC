// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Favalet;
using System.Collections.Generic;
using System.Text;

namespace Favalon
{
    public sealed class Parser
    {
        private enum States
        {
            Detect,
            String,
            StringEscaped,
            Numeric,
            Variable,
        }

        private readonly TextRange textRange;
        private States state = States.Detect;

        private Parser(TextRange textRange) =>
            this.textRange = textRange;

        public ParseResult Append(string text, int line) =>
            this.Append(text, Range.Create(Position.Create(line, 0), Position.Create(line, text.Length - 1)));

        public ParseResult Append(string text, Range range)
        {
            const char eol = '\xffff';

            var textRange = this.textRange.Subtract(range);
            var index = 0;
            var beginIndex = -1;
            StringBuilder? temp = null;
            Expression? expression = null;
            var errorInformations = new List<ParseErrorInformation>();
            char inch;

            TextRange GetCurrentTextRange() =>
                (beginIndex >= 0) ?
                    textRange.Subtract(Position.Create(textRange.Range.First.Line, beginIndex), Position.Create(textRange.Range.Last.Line, index - 1)) :
                    textRange.Subtract(Position.Create(textRange.Range.First.Line, index), Position.Create(textRange.Range.Last.Line, index));

            void RecordError(string details) =>
                errorInformations.Add(ParseErrorInformation.Create(details, GetCurrentTextRange()));

            do
            {
                inch = (index < text.Length) ? text[index] : eol;
                index++;
                switch (state)
                {
                    case States.Detect:
                        if (inch == '"')
                        {
                            beginIndex = index - 1;
                            state = States.String;
                        }
                        else if (char.IsDigit(inch))
                        {
                            beginIndex = index - 1;
                            state = States.Numeric;
                        }
                        else if (char.IsLetter(inch) || char.IsSymbol(inch))
                        {
                            beginIndex = index - 1;
                            state = States.Variable;
                        }
                        else if (char.IsWhiteSpace(inch) || (inch == eol))
                        {
                        }
                        else
                        {
                            RecordError("Invalid token at first.");
                        }
                        break;
                    case States.String:
                        if (inch == '"')
                        {
                            var literal = Expression.Literal(temp?.ToString() ?? string.Empty, GetCurrentTextRange());
                            expression += literal;
                            temp?.Clear();

                            beginIndex = -1;
                            state = States.Detect;
                        }
                        else if (inch == '\\')
                        {
                            state = States.StringEscaped;
                        }
                        else if (inch != eol)
                        {
                            temp ??= new StringBuilder();
                            temp.Append(inch);
                        }
                        else
                        {
                            RecordError("Invalid string token, reached end of line.");
                        }
                        break;
                    case States.StringEscaped:
                        if (inch != eol)
                        {
                            temp ??= new StringBuilder();
                            temp.Append(inch);
                            state = States.String;
                        }
                        else
                        {
                            RecordError("Invalid string escape, reached end of line.");
                        }
                        break;
                    case States.Numeric:
                        if (char.IsDigit(inch) || (inch == ',') || (inch == '.'))
                        {
                        }
                        else if (char.IsWhiteSpace(inch) || (inch == eol))
                        {
                            var numericString = text.Substring(beginIndex, index - beginIndex - 1);
                            var numeric =
                                int.TryParse(numericString, out var i) ? i : long.TryParse(numericString, out var l) ? l :
                                float.TryParse(numericString, out var f) ? f : double.Parse(numericString);
                            var literal = Expression.Literal(numeric, GetCurrentTextRange());
                            expression += literal;

                            beginIndex = -1;
                            state = States.Detect;
                        }
                        else
                        {
                            RecordError("Invalid numerical token at this location.");
                        }
                        break;
                    case States.Variable:
                        if (char.IsLetterOrDigit(inch) || char.IsSymbol(inch))
                        {
                        }
                        else if (char.IsWhiteSpace(inch) || (inch == eol))
                        {
                            var word = text.Substring(beginIndex, index - beginIndex - 1);
                            var variable = Expression.Free(word, Expression.Unspecified, GetCurrentTextRange());
                            expression += variable;

                            beginIndex = -1;
                            state = States.Detect;
                        }
                        else
                        {
                            RecordError("Invalid variable token at this location.");
                        }
                        break;
                }
            }
            while ((index <= text.Length) && (inch != eol));

            return ParseResult.Create(expression, errorInformations.ToArray()); ;
        }

        public static Parser Create() =>
            new Parser(TextRange.Create(Range.MaxValue));
        public static Parser Create(TextRange textRange) =>
            new Parser(textRange);
    }
}
