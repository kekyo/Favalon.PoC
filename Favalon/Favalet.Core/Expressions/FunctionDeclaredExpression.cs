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
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions
{
    public interface IFunctionDeclaredExpression :
        ITerm, IInferrableExpression, IReducibleExpression
    {
        IExpression Parameter { get; }
        IExpression Result { get; }
    }

    public static class FunctionDeclaredExpressionExtension
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Deconstruct(
            this IFunctionDeclaredExpression expression,
            out IExpression parameter,
            out IExpression result)
        {
            parameter = expression.Parameter;
            result = expression.Result;
        }
    }

    public sealed class FunctionDeclaredExpression :
        Expression, IFunctionDeclaredExpression
    {
        public readonly IExpression Parameter;
        public readonly IExpression Result;
        private readonly ValueLazy<IExpression> higherOrder;

        private FunctionDeclaredExpression(
            IExpression parameter, IExpression result, Func<IExpression> higherOrder)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.higherOrder = ValueLazy.Create(higherOrder);
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        IExpression IFunctionDeclaredExpression.Parameter =>
            this.Parameter;
        IExpression IFunctionDeclaredExpression.Result =>
            this.Result;

        public IExpression Infer(IInferContext context)
        {
            var parameter = this.Parameter.InferIfRequired(context);
            var result = this.Result.InferIfRequired(context);
            var higherOrder = this.HigherOrder.InferIfRequired(context);

            if (this.Parameter.ExactEquals(parameter) &&
                this.Result.ExactEquals(result) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionDeclaredExpression(parameter, result, () => higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var parameter = this.Parameter.ReduceIfRequired(context);
            var result = this.Result.ReduceIfRequired(context);
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);

            if (this.Parameter.ExactEquals(parameter) &&
                this.Result.ExactEquals(result) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionDeclaredExpression(parameter, result, () => higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is IFunctionDeclaredExpression functionDeclaration &&
            this.Parameter.Equals(functionDeclaration.Parameter) &&
            this.Result.Equals(functionDeclaration.Result);

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Result.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, this.Parameter, this.Result);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static FunctionDeclaredExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            new FunctionDeclaredExpression(
                parameter, result, () => higherOrder);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static FunctionDeclaredExpression Create(
            IExpression parameter, IExpression result) =>
            new FunctionDeclaredExpression(
                parameter, result, () => From(parameter.HigherOrder, result.HigherOrder));

        public static readonly FunctionDeclaredExpression Unspecified =
            Create(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance, TerminationTerm.Instance);

        public static readonly FunctionDeclaredExpression FourthType =
            Create(ExpressionFactory.fourthType, ExpressionFactory.fourthType, TerminationTerm.Instance);

        public static readonly FunctionDeclaredExpression KindType =
            Create(ExpressionFactory.kindType, ExpressionFactory.kindType, FourthType);

        public static IExpression From(IExpression parameter, IExpression result) =>
            (parameter, result) switch
            {
                (TerminationTerm _, TerminationTerm _) => TerminationTerm.Instance,
                (_, TerminationTerm _) => TerminationTerm.Instance,
                (TerminationTerm _, _) => TerminationTerm.Instance,
                (IIdentityTerm pid, IIdentityTerm rid) when
                    pid.Equals(UnspecifiedTerm.Instance) && rid.Equals(UnspecifiedTerm.Instance) => Unspecified,
                (IIdentityTerm pid, IIdentityTerm rid) when
                    pid.Equals(ExpressionFactory.fourthType) && rid.Equals(ExpressionFactory.fourthType) => FourthType,
                (IIdentityTerm pid, IIdentityTerm rid) when
                    pid.Equals(ExpressionFactory.kindType) && rid.Equals(ExpressionFactory.kindType) => KindType,
                _ => Create(parameter, result)
            };
    }
}
