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

namespace Favalon.Parsing.States
{
    internal sealed class StringState : State
    {
        private StringState()
        { }

        protected override Term MakeTerm(string token, TextRange textRange) =>
            Term.Literal(token, textRange);

        public override State Run(InteractiveInformation inch, StateContext context)
        {
            if (inch.Character == '"')
            {
                this.RunFinishing(context);
                return DetectState.Instance;
            }
            else if (inch.Character == '\\')
            {
                return StringEscapedState.Instance;
            }
            else
            {
                context.AppendToken(inch.Character);
                return this;
            }
        }

        public override void Finalize(StateContext context) =>
            context.RecordError("Invalid string token, reached end of line.");

        public static readonly State Instance = new StringState();
    }
}
