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
using Favalon.IO;
using System;
using System.Linq;

namespace Favalon.Parsing.States
{
    internal abstract class State
    {
        protected State() { }

        public static bool IsTokenSeparator(char ch) =>
            char.IsWhiteSpace(ch) || (ch == '\r') || (ch == '\n');

        protected virtual Term MakeTerm(string token, TextRange textRange) =>
            throw new InvalidOperationException();

        protected void RunFinishing(StateContext context)
        {
            var (token, textRange) = context.ExtractToken();
            context.AppendTerm(this.MakeTerm(token, textRange));
        }

        public abstract State Run(InteractiveInformation inch, StateContext context);

        public abstract void Finalize(StateContext context);

        public virtual ParseResult? PeekResult(StateContext context)
        {
            var (token, textRange) = context.PeekToken();
            var term = context.CurrentTerm + this.MakeTerm(token, textRange);
            return ParseResult.Create(term, context.CurrentErrors.ToArray(), context.GetCurrentTextRange());
        }
    }
}
