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

namespace Favalon.Parsing
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
}
