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
using Favalet.Terms;
using System.Collections.Generic;
using System.Text;

namespace Favalon.Parsing
{
    internal sealed class StateContext
    {
        public StateContext(TextRange textRange)
        {
            this.TextRange = textRange;
            currentPosition = textRange.Range.First;
        }

        public readonly TextRange TextRange;
        private Position currentPosition;
        private Position? startPosition;

        public Term? CurrentTerm { get; private set; }
        public IReadOnlyList<ParseErrorInformation> CurrentErrors =>
            errors;

        private readonly StringBuilder token = new StringBuilder();
        private readonly List<ParseErrorInformation> errors = new List<ParseErrorInformation>();

        public void BeginToken() =>
            startPosition = currentPosition;

        public void AppendTokenChar(char ch)
        {
            token.Append(ch);
            currentPosition += 1;
        }

        public void SkipTokenChar() =>
            currentPosition += 1;

        private TextRange CurrentTextRange =>
            this.TextRange.Subtract(Range.Create(startPosition is Position sp ? sp : currentPosition, currentPosition));

        public (string, TextRange) ExtractToken()
        {
            var textRange = this.CurrentTextRange;
            var token = this.token.ToString();

            this.token.Clear();
            startPosition = null;

            return (token, textRange);
        }

        public ParseResult? ExtractResult()
        {
            var term = this.CurrentTerm;
            var errors = this.errors.ToArray();

            if (term is Term || errors.Length >= 1)
            {
                this.CurrentTerm = null;
                this.errors.Clear();

                return ParseResult.Create(term, errors, null);
            }
            else
            {
                return null;
            }
        }

        public void RecordError(string details)
        {
            this.errors.Add(ParseErrorInformation.Create(true, details, this.CurrentTextRange));
            currentPosition += 1;
        }

        public void AppendTerm(Term term) =>
            this.CurrentTerm += term;
    }
}
