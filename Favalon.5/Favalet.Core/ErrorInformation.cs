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

using System.ComponentModel;

namespace Favalet
{
    public abstract class ErrorInformation
    {
        protected ErrorInformation(bool isError, string details)
        {
            this.IsError = isError;
            this.Details = details;
        }

        public readonly bool IsError;
        public readonly string Details;

        public abstract TextRange TextRange { get; }

        public override string ToString() =>
            $"{this.TextRange}: {this.Details}";

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out bool isError, out string details)
        {
            isError = this.IsError;
            details = this.Details;
        }
    }
}
