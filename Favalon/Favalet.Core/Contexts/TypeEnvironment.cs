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

using Favalet.Expressions;
using Favalet.Expressions.Comparer;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface ITypeEnvironment : ITypeContext
    {
        IExpression Infer(IExpression expression);

        IEnumerable<IExpression> Infer(IEnumerable<IExpression> expressions);
    }

    internal interface IRootTypeContext : ITypeContext
    {
        ITypeContextFeatures Features { get; }

        int DrawNextPlaceholderIndex();
    }

    public sealed class TypeEnvironment :
        TypeContext, ITypeEnvironment, IRootTypeContext
    {
        private int placeholderIndex;

        public readonly ITypeContextFeatures Features;
        public readonly int MaxIterationCount;

        private TypeEnvironment(ITypeContextFeatures features, int maxIterationCount) :
            base(null)
        {
            this.Features = features;
            this.MaxIterationCount = maxIterationCount;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ITypeContextFeatures IRootTypeContext.Features =>
            this.Features;

        int IRootTypeContext.DrawNextPlaceholderIndex() =>
            placeholderIndex++;

        public IExpression Infer(IExpression expression)
        {
            Debug.WriteLine($"Infer [0]: {expression}");

            if (expression is IInferrableExpression inferrable)
            {
                var context = InferContext.Create(this);

                var inferred = inferrable.Infer(context);
                var fixupped = inferred.FixupIfRequired(context);
                var current = fixupped;

                for (var index = 1; index < this.MaxIterationCount; index++)
                {
                    inferred = current.InferIfRequired(context);
                    fixupped = inferred.FixupIfRequired(context);

                    if (ExactEqualityComparer.Equals(current, fixupped))
                    {
                        Debug.WriteLine($"Infer [F]: {fixupped}");
                        return fixupped;
                    }
                    else
                    {
                        Debug.WriteLine($"Infer [{index}]: {fixupped}");
                        current = fixupped;
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

                    if (ExactEqualityComparer.Equals(current, reduced))
                    {
                        Debug.WriteLine($"Reduce [F]: {reduced}");
                        return reduced;
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
            new TypeEnvironment(TypeContextFeatures.Instance, maxIterationCount);

        public static TypeEnvironment Create(ITypeContextFeatures features, int maxIterationCount = 10000) =>
            new TypeEnvironment(features, maxIterationCount);
    }
}
