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
using System;
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
        private Expression? expression = null;

        private Parser(TextRange textRange) =>
            this.textRange = textRange;

        public Expression? Append(string text, int line) =>
            this.Append(text, Range.Create(Position.Create(line, 0), Position.Create(line, text.Length - 1)));

        public Expression? Append(string text, Range range)
        {
            const char eol = '\xffff';

            var textRange = this.textRange.Subtract(range);
            var index = 0;
            var beginIndex = -1;
            StringBuilder? temp = null;
            char inch;
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
                            throw new FormatException();
                        }
                        break;
                    case States.String:
                        if (inch == '"')
                        {
                            var literal = Expression.Literal(temp?.ToString() ?? string.Empty, textRange.Subtract((beginIndex, index - 1)));
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
                            throw new FormatException();
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
                            throw new FormatException();
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
                            var literal = Expression.Literal(numeric, textRange.Subtract((beginIndex, index - 1)));
                            expression += literal;

                            beginIndex = -1;
                            state = States.Detect;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                    case States.Variable:
                        if (char.IsLetterOrDigit(inch) || char.IsSymbol(inch))
                        {
                        }
                        else if (char.IsWhiteSpace(inch) || (inch == eol))
                        {
                            var word = text.Substring(beginIndex, index - beginIndex - 1);
                            var variable = Expression.Free(word, Expression.Unspecified, textRange.Subtract((beginIndex, index - 1)));
                            expression += variable;

                            beginIndex = -1;
                            state = States.Detect;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                }
            }
            while ((index <= text.Length) && (inch != eol));

            var e = expression;
            expression = null;

            return e;
        }

        public static Parser Create() =>
            new Parser(TextRange.Create(Range.MaxValue));
        public static Parser Create(TextRange textRange) =>
            new Parser(textRange);
    }
}
