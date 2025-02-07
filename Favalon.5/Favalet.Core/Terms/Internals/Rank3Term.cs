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

namespace Favalet.Terms.Specialized
{
    /// <summary>
    /// It's truly pseudo term usable only marking upper level of kind.
    /// </summary>
    internal sealed class Rank3Term : PseudoTerm
    {
        private Rank3Term() :
            base(null!, TextRange.Unknown)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Rank3)" : "#";

        internal static readonly Rank3Term Instance =
            new Rank3Term();
    }
}
