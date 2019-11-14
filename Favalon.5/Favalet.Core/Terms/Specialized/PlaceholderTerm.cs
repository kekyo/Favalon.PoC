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

namespace Favalet.Terms.Specialized
{
    public sealed class PlaceholderTerm :
        VariableTerm, IEquatable<PlaceholderTerm?>, IComparable<PlaceholderTerm>
    {
        internal PlaceholderTerm(int index, Term higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Index = index;

        public readonly int Index;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            switch (context.FormatNaming)
            {
                case FormatNamings.Friendly:
                    var index = context.GetAdjustedIndex(this);
                    var ch = (char)('a' + (index % ('z' - 'a' + 1)));
                    var suffixIndex = index / ('z' - 'a' + 1);
                    var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                    return $"'{ch}{suffix}";
                default:
                    return $"'{this.Index}";
            }
        }

        protected override Term VisitInferring(IInferringContext context, Term higherOrderHint) =>
            this;

        protected override Term VisitResolving(IResolvingContext context) =>
            (context.Lookup(this) is Term lookup) ? lookup : this;

        public override int GetHashCode() =>
            this.Index;

        public bool Equals(PlaceholderTerm? other) =>
            other?.Index.Equals(this.Index) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as PlaceholderTerm);

        public int CompareTo(PlaceholderTerm other) =>
            this.Index.CompareTo(other.Index);

        public void Deconstruct(out int index, out Term higherOrder, out TextRange textRange)
        {
            index = this.Index;
            higherOrder = this.HigherOrder;
            textRange = this.TextRange;
        }
    }
}
