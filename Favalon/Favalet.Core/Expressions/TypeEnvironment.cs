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
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ITypeEnvironment : ITypeContext
    {
        IExpression Infer(IExpression expression);

        IEnumerable<IExpression> Infer(IEnumerable<IExpression> expressions);
    }

    public sealed class TypeEnvironment :
        TypeContext, ITypeEnvironment, IRootTypeContext
    {
        public readonly IExpressionFactory Factory;
        public readonly int MaxIterationCount;

        private int placeholderIndex;

        private TypeEnvironment(IExpressionFactory factory, int maxIterationCount) :
            base(null)
        {
            this.Factory = factory;
            this.MaxIterationCount = maxIterationCount;
        }

        int IRootTypeContext.CreatePlaceholderIndex() =>
            placeholderIndex++;

        public IExpression Infer(IExpression expression)
        {
            Debug.WriteLine($"Infer [0]: {expression}");

            if (expression is IInferrableExpression inferrable)
            {
                var context = InferContext.Create(this, this);

                var current = inferrable.Infer(context);
                for (var index = 1; index < this.MaxIterationCount; index++)
                {
                    var inferred = current.InferIfRequired(context);
                    if (current.Equals(inferred))
                    {
                        Debug.WriteLine($"Infer [F]: {current}");
                        return current;
                    }
                    else
                    {
                        Debug.WriteLine($"Infer [{index}]: {inferred}");
                        current = inferred;
                    }
                }

                // Cannot finish inferring.
                throw new InvalidOperationException(
                    $"Cannot finish inferring: {expression}");
            }
            else
            {
                return expression;
            }
        }

        public IEnumerable<IExpression> Infer(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
            {
                yield return this.Infer(expression);
            }
        }

#if !NET35
        //public IObservable<IExpression> Infer(IObservable<IExpression> expressions)
        //{

        //}
#endif

        public IExpression Reduce(IExpression expression)
        {
            Debug.WriteLine($"Reduce [0]: {expression}");

            if (expression is IReducibleExpression reducible)
            {
                var context = ReduceContext.Create(this);

                var current = reducible.Reduce(context);
                for (var index = 1; index < this.MaxIterationCount; index++)
                {
                    var reduced = current.ReduceIfRequired(context);
                    if (current.Equals(reduced))
                    {
                        Debug.WriteLine($"Reduce [F]: {current}");
                        return current;
                    }
                    else
                    {
                        Debug.WriteLine($"Reduce [{index}]: {reduced}");
                        current = reduced;
                    }
                }

                // Cannot finish reducing.
                throw new InvalidOperationException(
                    $"Cannot finish reducing: {expression}");
            }
            else
            {
                return expression;
            }
        }

        public IEnumerable<IExpression> Reduce(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
            {
                yield return this.Reduce(expression);
            }
        }

#if !NET35
        //public IObservable<IExpression> Reduce(IObservable<IExpression> expressions)
        //{

        //}
#endif

        public static TypeEnvironment Create(int maxIterationCount = 10000) =>
            new TypeEnvironment(ExpressionFactory.Instance, maxIterationCount);

        public static TypeEnvironment Create(IExpressionFactory factory, int maxIterationCount = 10000) =>
            new TypeEnvironment(factory, maxIterationCount);
    }
}
