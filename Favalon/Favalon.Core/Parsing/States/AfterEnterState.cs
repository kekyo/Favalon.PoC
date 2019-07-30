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

using System;
using System.Runtime.CompilerServices;

namespace Favalon.Parsing.States
{
    internal sealed class AfterEnterState : State
    {
        private readonly char nextEnterChar;

        private AfterEnterState(char nextEnterChar) =>
            this.nextEnterChar = nextEnterChar;

        public override State Run(char inch, StateContext context)
        {
            if (inch == nextEnterChar)
            {
                context.SkipTokenChar();
                return DetectState.Instance;
            }
            else
            {
                return DetectState.Instance.Run(inch, context);
            }
        }

        public override void Finalize(StateContext context) =>
            context.SkipTokenChar();

        public static readonly State CarriageReturn = new AfterEnterState('\r');
        public static readonly State LineFeed = new AfterEnterState('\n');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State NextState(char ch) =>
            ch switch
            {
                '\r' => LineFeed,
                '\n' => CarriageReturn,
                _ => throw new InvalidOperationException()
            };
    }
}
