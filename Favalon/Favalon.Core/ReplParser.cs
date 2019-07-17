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
using System.IO;
using System.Threading.Tasks;

namespace Favalon
{
    public static class ReplParser
    {
        private enum States
        {
            Separation,
            AlphabeticalWord,
        }

        public static async Task<Expression> ParseAsync(TextReader tr)
        {
            Expression expression = null!;

            var state = States.Separation;
            while (true)
            {
                var line = await tr.InputAsync();
                var index = 0;
                var beginIndex = -1;
                while (index < line.Length)
                {
                    var ch = line[index++];
                    switch (state)
                    {
                        case States.Separation:
                            if (char.IsLetterOrDigit(ch) || (ch == '_'))
                            {
                                beginIndex = index;
                                state = States.AlphabeticalWord;
                            }
                            else if (char.IsWhiteSpace(ch))
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
                            else if (char.IsWhiteSpace(ch))
                            {
                                var word = line.Substring(beginIndex, index - beginIndex);
                                var freeVariable = Expression.Free(word);
                                expression = (expression != null) ? (Expression)Expression.Apply(expression, freeVariable) : freeVariable;

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
            }
        }

    }
}
