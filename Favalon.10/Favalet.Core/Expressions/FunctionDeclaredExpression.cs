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
using Favalet.Expressions.Comparer;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions
{
    public interface IFunctionDeclaredExpression :
        IInferrableExpression, IReducibleExpression
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
        Expression, IFunctionDeclaredExpression, IComparable<IExpression>
    {
        public readonly IExpression Parameter;
        public readonly IExpression Result;

        private FunctionDeclaredExpression(
            IExpression parameter, IExpression result, IExpression higherOrder)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionDeclaredExpression.Parameter =>
            this.Parameter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionDeclaredExpression.Result =>
            this.Result;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var parameter = this.Parameter.InferIfRequired(context);
            var result = this.Result.InferIfRequired(context);

            var functionDeclared = From(parameter.HigherOrder, result.HigherOrder);

            context.Unify(higherOrder, functionDeclared);

            if (this.Parameter.ExactEquals(parameter) &&
                this.Result.ExactEquals(result) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return From(parameter, result, higherOrder);
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var parameter = this.Parameter.FixupIfRequired(context);
            var result = this.Result.FixupIfRequired(context);
            var higherOrder = this.HigherOrder.FixupIfRequired(context);

            if (this.Parameter.ExactEquals(parameter) &&
                this.Result.ExactEquals(result) &&
                this.HigherOrder.ExactEquals(higherOrder))
            {
                return this;
            }
            else
            {
                return From(parameter, result, higherOrder);
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
                return From(parameter, result, higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs, IEqualityComparer<IExpression> comparer) =>
            rhs is IFunctionDeclaredExpression functionDeclaration &&
            comparer.Equals(this.Parameter, functionDeclaration.Parameter) &&
            comparer.Equals(this.Result, functionDeclaration.Result);

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Result.GetHashCode();

        int IComparable<IExpression>.CompareTo(IExpression rhs)
        {
            if (rhs is IFunctionDeclaredExpression fde)
            {
                if (ExpressionComparer.Compare(this.Parameter, fde.Parameter) is int rp && rp != 0)
                {
                    return rp;
                }
                if (ExpressionComparer.Compare(this.Result, fde.Result) is int rr && rr != 0)
                {
                    return rr;
                }
                return 0;
            }
            return this.GetHashCode().CompareTo(rhs.GetHashCode());
        }

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.Standard, this.Parameter, this.Result);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static FunctionDeclaredExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            new FunctionDeclaredExpression(
                parameter, result, higherOrder);

        public static readonly FunctionDeclaredExpression Unspecified =
            Create(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance, TerminationTerm.Instance);

        public static readonly FunctionDeclaredExpression KindType =
            Create(ExpressionFactory.kindType, ExpressionFactory.kindType, FourthTerm.Instance);

        private static IExpression From(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            (parameter, result) switch
            {
                (TerminationTerm _, TerminationTerm _) => TerminationTerm.Instance,
                (_, TerminationTerm _) => TerminationTerm.Instance,
                (TerminationTerm _, _) => TerminationTerm.Instance,
                (UnspecifiedTerm _, UnspecifiedTerm _)  => Unspecified,
                (FourthTerm _, FourthTerm _) => FourthTerm.Instance,
                (IIdentityTerm pid, IIdentityTerm rid) when
                    pid.Equals(ExpressionFactory.kindType) && rid.Equals(ExpressionFactory.kindType) => KindType,
                _ => Create(parameter, result, higherOrder)
            };

        public static IExpression From(IExpression parameter, IExpression result) =>
            From(parameter, result, UnspecifiedTerm.Instance);
    }
}
