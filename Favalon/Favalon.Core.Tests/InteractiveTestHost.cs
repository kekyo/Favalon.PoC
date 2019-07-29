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
using Favalon.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public struct InputElements
    {
        public readonly InteractiveInformation[] Inputs;

#line hidden
        private InputElements(InteractiveInformation[] inputs) =>
            this.Inputs = inputs;
        private InputElements(string inputString, InteractiveModifiers modifier) =>
            this.Inputs = inputString.Select(ch => InteractiveInformation.Create(ch, modifier)).ToArray();

        public static InputElements Create(string inputString) =>
            new InputElements(inputString, InteractiveModifiers.None);

        public static InputElements Create(string inputString, InteractiveModifiers modifier) =>
            new InputElements(inputString, modifier);

        public static implicit operator InputElements(string inputString) =>
            new InputElements(inputString, InteractiveModifiers.None);
        public static implicit operator InputElements((string inputString, InteractiveModifiers modifier) input) =>
            new InputElements(input.inputString, input.modifier);

        public static InputElements operator +(InputElements lhs, InputElements rhs) =>
            new InputElements(lhs.Inputs.Concat(rhs.Inputs).ToArray());
#line default
    }

    public sealed class InteractiveTestHost : InteractiveHost
    {
        private readonly InputElements elements;
        private readonly Queue<Action<LogLevels?, string>> assertions = new Queue<Action<LogLevels?, string>>();
        private readonly StringBuilder text = new StringBuilder();

        private InteractiveTestHost(InputElements[] elements) :
            base(TextRange.Create("test", Range.MaxLength)) =>
            this.elements = elements.Aggregate((lhs, rhs) => lhs + rhs);

        public InteractiveTestHost Assert(Action<LogLevels?, string> assert)
        {
            assertions.Enqueue(assert);
            return this;
        }

        public void Run()
        {
            foreach (var input in this.elements.Inputs)
            {
                base.OnNext(input);
            }

            base.OnCompleted();
        }

        public override void Write(char ch) =>
            this.text.Append(ch);

        public override void Write(string text) =>
            this.text.Append(text);

        public override void WriteLine()
        {
            assertions.Dequeue().Invoke(null, this.text.ToString());
            this.text.Clear();
        }

        public override void WriteLog(LogLevels level, string text)
        {
            if (this.text.Length >= 1)
            {
                assertions.Dequeue().Invoke(null, this.text.ToString());
                this.text.Clear();
            }
            assertions.Dequeue().Invoke(level, text);
        }

        public static InteractiveTestHost Create(params InputElements[] elements) =>
            new InteractiveTestHost(elements);
    }
}
