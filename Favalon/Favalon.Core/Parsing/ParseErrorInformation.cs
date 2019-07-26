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
using System.ComponentModel;

namespace Favalon.Parsing
{
    public sealed class ParseErrorInformation : ErrorInformation
    {
        private ParseErrorInformation(string details, TextRange textRange) :
            base(details) =>
            this.TextRange = textRange;

        public override TextRange TextRange { get; }

        public static ParseErrorInformation Create(string details, TextRange textRange) =>
            new ParseErrorInformation(details, textRange);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out string details, out TextRange textRange)
        {
            details = this.Details;
            textRange = this.TextRange;
        }
    }
}
