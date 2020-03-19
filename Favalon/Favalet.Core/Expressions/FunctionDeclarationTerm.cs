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
using System;

namespace Favalet.Expressions
{
    public interface IFunctionDeclarationTerm :
        ITerm, IInferrableExpression, IReducibleExpression
    {
        IExpression Parameter { get; }
        IExpression Result { get; }
    }

    public sealed class FunctionDeclarationTerm :
        Term, IFunctionDeclarationTerm
    {
        public readonly IExpression Parameter;
        public readonly IExpression Result;
        private readonly Lazy<IExpression> higherOrder;

        private FunctionDeclarationTerm(
            IExpression parameter, IExpression result)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.higherOrder = new Lazy<IExpression>(() =>
                From(this.Parameter.HigherOrder, this.Result.HigherOrder));
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        IExpression IFunctionDeclarationTerm.Parameter =>
            this.Parameter;
        IExpression IFunctionDeclarationTerm.Result =>
            this.Result;

        public IExpression Infer(IInferContext context)
        {
            var parameter = this.Parameter.InferIfRequired(context);
            var result = this.Result.InferIfRequired(context);
            var higherOrder = this.HigherOrder.InferIfRequired(context);

            if (this.Parameter.Equals(parameter) &&
                this.Result.Equals(result) &&
                this.HigherOrder.Equals(higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionDeclarationTerm(parameter, result, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var parameter = this.Parameter.ReduceIfRequired(context);
            var result = this.Result.ReduceIfRequired(context);
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);

            if (this.Parameter.Equals(parameter) &&
                this.Result.Equals(result) &&
                this.HigherOrder.Equals(higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionDeclarationTerm(parameter, result, higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is IFunctionDeclarationTerm functionDeclaration &&
            this.Parameter.Equals(functionDeclaration.Parameter) &&
            this.Result.Equals(functionDeclaration.Result);

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Result.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, this.Parameter, this.Result);

        public static readonly FunctionDeclarationTerm Kind =
            new FunctionDeclarationTerm(ExpressionFactory.KindType(), ExpressionFactory.KindType());

        public static FunctionDeclarationTerm From(IExpression parameter, IExpression result) =>
            (parameter, result) switch
            {
                ()
            };
    }
}
