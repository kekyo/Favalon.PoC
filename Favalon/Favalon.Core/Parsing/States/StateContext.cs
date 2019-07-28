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

namespace Favalon.Parsing.States
{
    internal sealed class StateContext
    {
        public StateContext(TextRange textRange) =>
            this.TextRange = textRange;

        public readonly TextRange TextRange;

        public Position CurrentPosition { get; private set; }
        public Position? StartPosition { get; private set; }
        public Term? CurrentTerm { get; private set; }
        public IReadOnlyList<ParseErrorInformation> CurrentErrors =>
            errors;

        private readonly StringBuilder token = new StringBuilder();
        private readonly List<ParseErrorInformation> errors = new List<ParseErrorInformation>();

        public void AppendTokenChar(char ch, Position newPosition)
        {
            this.token.Append(ch);
            this.CurrentPosition = newPosition;
        }

        public void SkipTokenChar(Position newPosition) =>
            this.CurrentPosition = newPosition;

        public void RecordStartPosition() =>
            this.StartPosition = this.CurrentPosition;

        public TextRange GetCurrentTextRange() =>
            this.TextRange.Subtract(
                this.StartPosition is Position startPosition ? startPosition : this.CurrentPosition, this.CurrentPosition);

        public (string, TextRange) ExtractToken()
        {
            var textRange = this.GetCurrentTextRange();
            var token = this.token.ToString();

            this.token.Clear();
            this.StartPosition = null;

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

        public (string, TextRange) PeekToken()
        {
            var textRange = this.GetCurrentTextRange();
            var token = this.token.ToString();

            return (token, textRange);
        }

        public void RecordError(string details, Position? newPosition)
        {
            this.errors.Add(ParseErrorInformation.Create(details, this.GetCurrentTextRange()));
            this.StartPosition = newPosition;
        }

        public void AppendTerm(Term term) =>
            this.CurrentTerm += term;
    }
}
