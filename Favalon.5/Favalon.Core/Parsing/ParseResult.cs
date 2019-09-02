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
using System.ComponentModel;

namespace Favalon.Parsing
{
    public struct ParseResult
    {
        public readonly Term? Term;
        public readonly ParseErrorInformation[] ErrorInformations;
        public readonly TextRange? TargetTextRange;

        private ParseResult(Term? term, ParseErrorInformation[] errorInformations, TextRange? targetTextRange)
        {
            this.Term = term;
            this.ErrorInformations = errorInformations;
            this.TargetTextRange = targetTextRange;
        }

        public static ParseResult Create(Term? term, ParseErrorInformation[] errorInformations, TextRange? targetTextRange) =>
            new ParseResult(term, errorInformations, targetTextRange);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Term? term, out ParseErrorInformation[] errorInformations, out TextRange? targetTextRange)
        {
            term = this.Term;
            errorInformations = this.ErrorInformations;
            targetTextRange = this.TargetTextRange;
        }
    }
}
