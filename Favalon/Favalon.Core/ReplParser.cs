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

using Favalet.Expressions;
using System;
using System.Globalization;

namespace Favalon
{
    public sealed class ReplParser
    {
        private enum States
        {
            Separation,
            AlphabeticalWord,
            Numeric,
            Symbol
        }

        private States state = States.Separation;
        private Expression? expression = null;

        public ReplParser()
        { }

        public Expression? Append(string text)
        {
            var index = 0;
            var beginIndex = -1;
            char ch;
            do
            {
                ch = (index < text.Length) ? text[index] : '\xffff';
                index++;
                switch (state)
                {
                    case States.Separation:
                        if (char.IsLetter(ch) || (ch == '_'))
                        {
                            beginIndex = index - 1;
                            state = States.AlphabeticalWord;
                        }
                        else if (char.IsDigit(ch))
                        {
                            beginIndex = index - 1;
                            state = States.Numeric;
                        }
                        else if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.MathSymbol)
                        {
                            beginIndex = index - 1;
                            state = States.Symbol;
                        }
                        else if (char.IsWhiteSpace(ch) || (ch == '\xffff'))
                        {
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                    case States.AlphabeticalWord:
                        if (char.IsLetterOrDigit(ch) || (ch == '_'))
                        {
                        }
                        else if (char.IsWhiteSpace(ch) || (ch == '\xffff'))
                        {
                            var word = text.Substring(beginIndex, index - beginIndex - 1);
                            var freeVariable = Expression.Free(word);
                            expression = expression.Apply(freeVariable);

                            beginIndex = -1;
                            state = States.Separation;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                    case States.Numeric:
                        if (char.IsDigit(ch) || (ch == ',') || (ch == '.'))
                        {
                        }
                        else if (char.IsWhiteSpace(ch) || (ch == '\xffff'))
                        {
                            var numericString = text.Substring(beginIndex, index - beginIndex - 1);
                            var numeric =
                                int.TryParse(numericString, out var i) ? i : long.TryParse(numericString, out var l) ? l :
                                float.TryParse(numericString, out var f) ? f : double.Parse(numericString);
                            var literal = Expression.Literal(numeric);
                            expression = expression.Apply(literal);

                            beginIndex = -1;
                            state = States.Separation;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                    case States.Symbol:
                        if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.MathSymbol)
                        {
                        }
                        else if (char.IsWhiteSpace(ch) || (ch == '\xffff'))
                        {
                            var word = text.Substring(beginIndex, index - beginIndex - 1);
                            var freeVariable = Expression.Free(word);
                            expression = expression.Apply(freeVariable);

                            beginIndex = -1;
                            state = States.Separation;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                        break;
                }
            }
            while ((index <= text.Length) && (ch != '\uffff'));

            var e = expression;
            expression = null;

            return e;
        }
    }
}
