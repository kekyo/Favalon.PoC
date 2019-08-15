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

namespace Favalon.Parsing.States
{
    internal sealed class VariableState : State
    {
        private VariableState()
        { }

        protected override Term MakeTerm(string token, TextRange textRange) =>
            Term.Free(token, Term.Unspecified, textRange);

        public override State Run(char inch, StateContext context)
        {
            if (char.IsLetterOrDigit(inch) || Utilities.IsDeclarableOperator(inch))
            {
                context.AppendTokenCharAndForward(inch);
                return this;
            }
            else if (Utilities.IsEnter(inch))
            {
                this.RunFinishing(context);
                context.SkipTokenChar();
                return AfterEnterState.NextState(inch);
            }
            else if (char.IsWhiteSpace(inch))
            {
                this.RunFinishing(context);
                context.SkipTokenChar();
                return DetectState.Instance;
            }
            else
            {
                context.RecordError("Invalid variable token at this location.");
                return SkipState.Instance;
            }
        }

        public override void Finalize(StateContext context) =>
            this.RunFinishing(context);

        public static readonly State Instance = new VariableState();
    }
}
