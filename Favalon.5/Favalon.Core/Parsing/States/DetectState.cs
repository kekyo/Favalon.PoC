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

using Favalet.Terms;

namespace Favalon.Parsing.States
{
    internal sealed class DetectState : State
    {
        private DetectState()
        { }

        public override (State state, StateContext context) Run(char inch, StateContext context)
        {
            if (inch == '"')
            {
                return (StringState.Instance, context.Forward());
            }
            else if (char.IsDigit(inch))
            {
                return (NumericState.Instance, context.AppendTokenCharAndForward(inch));
            }
            else if (char.IsLetter(inch) || Utilities.IsDeclarableOperator(inch))
            {
                return (VariableState.Instance, context.AppendTokenCharAndForward(inch));
            }
            else if (Utilities.IsEnter(inch))
            {
                return (AfterEnterState.NextState(inch), context.Forward());
            }
            else if (char.IsWhiteSpace(inch))
            {
                return (this, context.Forward());
            }
            else
            {
                return (this, context.RecordError("Invalid token at first."));
            }
        }

        public override Term? FinalizeTerm(StateContext context) =>
            context.CombineTerm(null);

        public static readonly State Instance = new DetectState();
    }
}
