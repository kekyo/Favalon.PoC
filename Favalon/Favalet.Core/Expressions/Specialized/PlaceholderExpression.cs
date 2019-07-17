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

namespace Favalet.Expressions.Specialized
{
    public sealed class PlaceholderExpression :
        VariableExpression, IEquatable<PlaceholderExpression?>, IComparable<PlaceholderExpression>
    {
        internal PlaceholderExpression(int index, Expression higherOrder, TextRange textRange) :
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

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this;

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            (environment.Lookup(this) is Expression lookup) ? lookup : this;

        public override int GetHashCode() =>
            this.Index;

        public bool Equals(PlaceholderExpression? other) =>
            other?.Index.Equals(this.Index) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as PlaceholderExpression);

        public int CompareTo(PlaceholderExpression other) =>
            this.Index.CompareTo(other.Index);
    }
}
