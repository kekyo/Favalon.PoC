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

        IExpression IApplyExpression.Function =>
            this.Function;

        IExpression IApplyExpression.Argument =>
            this.Argument;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var argument = this.Argument.InferIfRequired(context);
            var function = this.Function.InferIfRequired(context);

            //context.Unify();

            if (this.Function.Equals(function) &&
                this.Argument.Equals(argument) &&
                this.HigherOrder.Equals(higherOrder))
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

            if (this.Function.Equals(function) &&
                this.Argument.Equals(argument) &&
                this.HigherOrder.Equals(higherOrder))
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

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, this.Function, this.Argument);

        public static ApplyExpression Create(
            IExpression function, IExpression argument, IExpression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
    }
}
