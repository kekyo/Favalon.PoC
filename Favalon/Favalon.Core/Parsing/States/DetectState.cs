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

namespace Favalon.Parsing.States
{
    internal sealed class DetectState : State
    {
        private DetectState()
        { }

        public override State Run(char inch, StateContext context)
        {
            if (inch == '"')
            {
                context.BeginToken();
                context.SkipTokenChar();
                return StringState.Instance;
            }
            else if (char.IsDigit(inch))
            {
                context.BeginToken();
                context.AppendTokenChar(inch);
                return NumericState.Instance;
            }
            else if (char.IsLetter(inch) || Utilities.IsDeclarableOperator(inch))
            {
                context.BeginToken();
                context.AppendTokenChar(inch);
                return VariableState.Instance;
            }
            else if (Utilities.IsEnter(inch))
            {
                context.SkipTokenChar();
                return AfterEnterState.NextState(inch);
            }
            else if (char.IsWhiteSpace(inch))
            {
                context.SkipTokenChar();
                return this;
            }
            else
            {
                context.RecordError("Invalid token at first.");
                return this;
            }
        }

        public override void Finalize(StateContext context) =>
            context.SkipTokenChar();

        public static readonly State Instance = new DetectState();
    }
}
