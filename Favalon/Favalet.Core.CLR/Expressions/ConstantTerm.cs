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
using Favalet.Internal;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Favalet.Expressions
{
    public sealed class ConstantTerm :
        Expression, IConstantTerm
    {
        public readonly object Value;
        private readonly ValueLazy<ConstantTerm, ITerm> higherOrder;

        private ConstantTerm(object value)
        {
            this.Value = value;
            this.higherOrder = ValueLazy.Create(
                this,
                @this => TypeTerm.From(@this.Value.GetType()));
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IConstantTerm.Value =>
            this.Value;

        public override bool Equals(IExpression? rhs) =>
            rhs is IConstantTerm constant &&
                this.Value.Equals(constant.Value);

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(
                this,
                (this.Value.GetType().IsPrimitive() || (this.Value is string)) ?
                    FormatOptions.ForceText | FormatOptions.SuppressHigherOrder :
                    FormatOptions.Standard,
                this.Value switch
                {
                    string str => $"\"{str}\"",
                    _ => this.Value.ToString()
                });

        public static ITerm From(object value) =>
            value switch
            {
                char ch => new SingleCharConstantTerm(ch, UnspecifiedTerm.Instance),
                string str when str.Length == 1 => new SingleCharConstantTerm(str[0], UnspecifiedTerm.Instance),
                _ => new ConstantTerm(value)
            };
    }

    public sealed class SingleCharConstantTerm :
        Expression, IConstantTerm, IInferrableExpression
    {
        private static readonly IExpression charTerm = TypeTerm.From(typeof(char));
        private static readonly IExpression higherOrder =
            SumExpression.Create(new[] { charTerm, TypeTerm.From(typeof(string)) }, ExpressionFactory.kindType);

        public readonly char Value;

        internal SingleCharConstantTerm(char value, IExpression higherOrder)
        {
            this.Value = value;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IConstantTerm.Value =>
            this.HigherOrder.Equals(charTerm) ? (object)this.Value : this.Value.ToString();

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);

            context.Unify(
                SingleCharConstantTerm.higherOrder,
                higherOrder);

            if (this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new SingleCharConstantTerm(this.Value, higherOrder);
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
                return new SingleCharConstantTerm(this.Value, higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is IConstantTerm constant &&
                this.Value.Equals(constant.Value);

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(
                this,
                FormatOptions.ForceText | FormatOptions.SuppressHigherOrder,
                $"\"{this.Value}\"");
    }
}
