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

using Favalet.Terms;
using Favalon.IO;

namespace Favalon.Parsing.States
{
    internal sealed class NumericState : State
    {
        private NumericState()
        { }

        private void RunFinishing(StateContext context)
        {
            var (numericString, textRange) = context.ExtractToken();
            var numeric =
                int.TryParse(numericString, out var i) ? i : long.TryParse(numericString, out var l) ? l :
                float.TryParse(numericString, out var f) ? f : double.Parse(numericString);

            var literal = Term.Literal(numeric, textRange);
            context.AppendTerm(literal);
        }

        public override State Run(InteractiveInformation inch, StateContext context)
        {
            if (char.IsDigit(inch.Character) || (inch.Character == '.'))
            {
                context.AppendToken(inch.Character);
                return this;
            }
            else if (inch.Character == '_')
            {
                return this;
            }
            else if (IsTokenSeparator(inch.Character))
            {
                this.RunFinishing(context);
                return DetectState.Instance;
            }
            else
            {
                context.RecordError("Invalid numerical token at this location.");
                return SkipState.Instance;
            }
        }

        public override void Finalize(StateContext context) =>
            this.RunFinishing(context);

        public static readonly State Instance = new NumericState();
    }
}
