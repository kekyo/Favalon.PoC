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
using Favalet.Expressions.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public enum TryCallResultContains
    {
        CalledResult,
        InterpretedArgument
    }

    public interface ICallableExpression : IExpression
    {
        TryCallResultContains TryCall(
            IReduceContext context, IExpression argument, out IExpression result);
    }

    public interface IApplyExpression : IInferrableExpression, IReducibleExpression
    {
        IExpression Function { get; }
        IExpression Argument { get; }
    }

    public sealed class ApplyExpression : Expression, IApplyExpression
    {
        public readonly IExpression Function;
        public readonly IExpression Argument;

        private ApplyExpression(
            IExpression function, IExpression argument, IExpression higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IApplyExpression.Function =>
            this.Function;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IApplyExpression.Argument =>
            this.Argument;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var argument = this.Argument.InferIfRequired(context);
            var function = this.Function.InferIfRequired(context);

            var functionDeclaration = FunctionDeclaredExpression.From(
                argument.HigherOrder, higherOrder,
                context.CreatePlaceholder);

            context.Unify(functionDeclaration, function.HigherOrder);

            if (this.Function.ExactEquals(function) &&
                this.Argument.ExactEquals(argument) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new ApplyExpression(function, argument, higherOrder);
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var argument = this.Argument.FixupIfRequired(context);
            var function = this.Function.FixupIfRequired(context);

            if (this.Function.ExactEquals(function) &&
                this.Argument.ExactEquals(argument) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new ApplyExpression(function, argument, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            IExpression higherOrder;
            IExpression argument;
            IExpression function;

            if (this.Function is ICallableExpression cf)
            {
                if (cf.TryCall(context, this.Argument, out var result) ==
                    TryCallResultContains.CalledResult)
                {
                    return result;
                }
                else
                {
                    higherOrder = this.HigherOrder.ReduceIfRequired(context);
                    argument = result;
                    function = this.Function.ReduceIfRequired(context);
                }
            }
            else
            {
                higherOrder = this.HigherOrder.ReduceIfRequired(context);
                argument = this.Argument.ReduceIfRequired(context);
                function = this.Function.ReduceIfRequired(context);
            }

            if (this.Function.ExactEquals(function) &&
                this.Argument.ExactEquals(argument) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new ApplyExpression(function, argument, higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is IApplyExpression apply &&
                this.Function.Equals(apply.Function) &&
                this.Argument.Equals(apply.Argument);

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.Standard, this.Function, this.Argument);

        public static ApplyExpression Create(
            IExpression function, IExpression argument, IExpression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
    }
}
