////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Contexts;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Specialized
{
    public sealed class PlaceholderTerm : Expression, IIdentityTerm
    {
        public readonly int Index;

        private PlaceholderTerm(int index, IExpression higherOrder)
        {
            this.Index = index;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        string IIdentityTerm.Identity =>
            $"'{this.Index}";

        public IExpression Infer(IInferContext context)
        {
            // Special case: will not infer Unspecified higherOrder.
            //   Maybe it makes infinity digging.
            if (this.HigherOrder is UnspecifiedTerm)
            {
                return this;
            }

            var higherOrder = this.HigherOrder.InferIfRequired(context);
            if (this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new PlaceholderTerm(this.Index, higherOrder);
            }
        }

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override bool Equals(IExpression? rhs) =>
            rhs is PlaceholderTerm placeholder &&
            this.Index.Equals(placeholder.Index);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            $"'{context.GetPlaceholderIndexString(this.Index)}";

        internal static PlaceholderTerm Create(int index, IExpression higherOrder) =>
            new PlaceholderTerm(index, higherOrder);
    }
}
