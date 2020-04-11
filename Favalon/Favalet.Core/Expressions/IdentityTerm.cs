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
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions
{
    public interface IIdentityTerm :
        ITerm
    {
        string Identity { get; }
    }

    public sealed class IdentityTerm :
        Expression, IIdentityTerm, IInferrableExpression, IReducibleExpression
    {
        public readonly string Identity;

        private IdentityTerm(string identity, IExpression higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Identity =>
            this.Identity;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);

            if (context.Lookup(this) is BoundInformation[] bounds && (bounds.Length >= 1))
            {
                // TODO: bound attributes

                var overloads = OverloadTerm.From(
                    bounds.Select(bound => bound.Expression))!;
                var inferred = overloads.InferIfRequired(context, 2);

                context.Unify(higherOrder, inferred.HigherOrder);

                return inferred;
            }

            if (this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new IdentityTerm(this.Identity, higherOrder);
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            if (this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new IdentityTerm(this.Identity, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var bounds = context.Lookup(this);
            if (bounds.Length >= 1)
            {
                // TODO: bound attributes
                return SumExpression.From(bounds.Select(bound => bound.Expression), false)?.
                    ReduceIfRequired(context) ?? TerminationTerm.Instance;
            }
            else
            {
                var higherOrder = this.HigherOrder.ReduceIfRequired(context);
                if (this.HigherOrder.ExactEquals(higherOrder))
                {
                    return this;
                }
                else
                {
                    return new IdentityTerm(this.Identity, higherOrder);
                }
            }
        }

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override bool Equals(IExpression? rhs) =>
            rhs is IIdentityTerm identity &&
                this.Identity.Equals(identity.Identity);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.UseSimpleLabel ? this.Identity : context.Format(this, this.Identity);

        public static IdentityTerm Create(string identity, IExpression higherOrder) =>
            new IdentityTerm(identity, higherOrder);
    }
}
