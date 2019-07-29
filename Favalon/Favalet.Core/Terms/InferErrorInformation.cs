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

namespace Favalet.Terms
{
    public sealed class InferErrorInformation : ErrorInformation
    {
        private InferErrorInformation(bool isError, string details, Term primaryTerm, Term[] terms) :
            base(isError, details)
        {
            this.PrimaryTerm = primaryTerm;
            this.Terms = terms;
        }

        public readonly Term PrimaryTerm;
        public readonly Term[] Terms;

        public override TextRange TextRange =>
            this.PrimaryTerm.TextRange;

        public static InferErrorInformation Create(bool isError, string details, Term primaryTerm, params Term[] terms) =>
            new InferErrorInformation(isError, details, primaryTerm, terms);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out bool isError, out string details, out Term primaryTerm, out Term[] terms)
        {
            isError = this.IsError;
            details = this.Details;
            primaryTerm = this.PrimaryTerm;
            terms = this.Terms;
        }
    }
}
