﻿// This is part of Favalon project - https://github.com/kekyo/Favalon
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
    internal sealed class VariableState : State
    {
        private VariableState()
        { }

        protected override Term MakeTerm(string token, TextRange textRange) =>
            Term.Free(token, Term.Unspecified, textRange);

        public override State Run(InteractiveInformation inch, StateContext context)
        {
            if (char.IsLetterOrDigit(inch.Character) || Utilities.IsDeclarableOperator(inch.Character))
            {
                context.AppendTokenChar(inch.Character);
                return this;
            }
            else if (Utilities.IsEnter(inch.Character))
            {
                this.RunFinishing(context);
                context.ForwardToken();
                return AfterEnterState.NextState(inch.Character);
            }
            else if (char.IsWhiteSpace(inch.Character))
            {
                this.RunFinishing(context);
                context.ForwardToken();
                return DetectState.Instance;
            }
            else
            {
                context.RecordError("Invalid variable token at this location.", context.CurrentPosition + 1);
                return SkipState.Instance;
            }
        }

        public override void Finalize(StateContext context) =>
            this.RunFinishing(context);

        public static readonly State Instance = new VariableState();
    }
}
