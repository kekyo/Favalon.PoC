﻿////////////////////////////////////////////////////////////////////////////
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
    public sealed class PlaceholderTerm :
        Expression, ITerm, IInferrableExpression, IReducibleExpression
    {
        public readonly int Index;

        private PlaceholderTerm(int index, IExpression higherOrder)
        {
            this.Index = index;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        public IExpression Infer(IInferContext context)
        {
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

        public IExpression Fixup(IFixupContext context)
        {
            if (context.LookupPlaceholder(this) is IExpression resolved)
            {
                return resolved.FixupIfRequired(context);
            }

            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            if (this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new PlaceholderTerm(this.Index, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);
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

        public override T Format<T>(IFormatContext<T> context) =>
//            context.Format(this, FormatOptions.ForceText, $"'{context.GetPlaceholderIndexString(this.Index)}");
            context.Format(this, FormatOptions.ForceText, $"'{this.Index}");

        internal static PlaceholderTerm Create(IInternalInferContext context, int currentOrder)
        {
            var index = context.DrawNextPlaceholderIndex();

            if (currentOrder <= 2)
            {
                return new PlaceholderTerm(
                    index,
                    Create(context, currentOrder + 1));
            }
            else
            {
                return new PlaceholderTerm(
                    index,
                    TerminationTerm.Instance);
            }
        }
    }
}
