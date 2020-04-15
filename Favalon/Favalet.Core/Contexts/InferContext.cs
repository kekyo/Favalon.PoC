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
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts
{
    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        IPlaceholderTerm CreatePlaceholder(IExpression higherOrder);

        void Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        IExpression? Widen(IExpression to, IExpression from);

        IExpression? Resolve(IPlaceholderTerm placeholder);
    }

    internal sealed class InferContext :
        TypeContext, IInferContext, IFixupContext
    {
#if DEBUG
        private int recursiveCount = 0;
        private int breakIndex = 5;
#endif

        private readonly IRootTypeContext rootContext;
        private readonly Dictionary<int, IExpression> descriptions;

        private InferContext(IRootTypeContext rootContext) :
            base(rootContext)
        {
            this.rootContext = rootContext;
            this.descriptions = new Dictionary<int, IExpression>();
        }

        private InferContext(InferContext parent) :
            base(parent)
        {
            this.rootContext = parent.rootContext;
            this.descriptions = parent.descriptions;
        }

        public IInferContext CreateDerivedScope() =>
            new InferContext(this);

        public IPlaceholderTerm CreatePlaceholder(IExpression higherOrder) =>
            PlaceholderTerm.Create(this.rootContext.DrawNextPlaceholderIndex(), higherOrder);

        private IExpression? InternalUnifyPlaceholder(
            IPlaceholderTerm placeholder,
            IExpression from,
            bool isForward,
            bool overloadable,
            bool isWiden)
        {
            Debug.Assert(
                from is PlaceholderTerm ||
                !(from.HigherOrder is UnspecifiedTerm));

            Debug.Assert(placeholder.Index != breakIndex);

            Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}]: Placeholder={placeholder}, From={from}, IsForward={isForward}, Overloadable={overloadable}, IsWiden={isWiden}");

            if (this.descriptions.TryGetValue(placeholder.Index, out var lastCombined))
            {
                Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}-1]: {lastCombined}");

                var result = isForward ?
                    this.InternalUnify(lastCombined, from, overloadable, isWiden) :  // forward
                    this.InternalUnify(from, lastCombined, overloadable, isWiden);   // backward

                if (result is IExpression)
                {
                    Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}-2]: {result}");

                    this.descriptions[placeholder.Index] = result;
                    return result;
                }
                else if (overloadable && !isWiden)
                {
                    var combinedExpression = OverloadTerm.From(new[] { lastCombined, from });

                    Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}-3]: {combinedExpression}");

                    this.descriptions[placeholder.Index] = combinedExpression!;
                    return combinedExpression;
                }
                else
                {
                    Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}-4]");

                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"UnifyPlaceholder [{recursiveCount}-5]");

                this.descriptions.Add(placeholder.Index, from);
                return from;
            }
        }

        private IExpression? InternalUnify(
            IExpression to, IExpression from,
            bool overloadable, bool isWiden)
        {
            Debug.WriteLine($"Unify [{++recursiveCount}]: To={to}, From={from}, Overloadable={overloadable}, IsWiden={isWiden}");

            // int: int <-- int
            // IComparable: IComparable <-- IComparable
            // _[1]: _[1] <-- _[1]
            if (to.ExactEquals(from))
            {
                Debug.WriteLine($"Unify [{recursiveCount--}-1]");

                return to;
            }

            // int->object: int->object <-- object->int
            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                Debug.WriteLine($"Unify [{recursiveCount}-2]: TP={tp}, TR={tr}, FP={fp}, FR={fr}");

                var pr = this.InternalUnify(fp, tp, false, isWiden);

                Debug.WriteLine($"Unify [{recursiveCount}-3]: PR={pr}");

                var rr = this.InternalUnify(tr, fr, false, isWiden);

                if (pr != null && rr != null)
                {
                    Debug.WriteLine($"Unify [{recursiveCount--}-4]: RR={rr}");

                    return FunctionDeclaredExpression.From(pr, rr);
                }
                else
                {
                    Debug.WriteLine($"Unify [{recursiveCount--}-5]: RR={rr}");

                    return null;
                }
            }

            // (int + double): (int + double) <-- (int + double)
            // (int + double + string): (int + double + string) <-- (int + double)
            // (int + IComparable): (int + IComparable) <-- (int + string)
            // null: (int + double) <-- (int + double + string)
            // null: (int + IServiceProvider) <-- (int + double)
            // (int + _): (int + _) <-- (int + string)
            // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
            if (to is ISumExpression(IExpression[] tss1) &&
                from is ISumExpression(IExpression[] fss1))
            {
                Debug.WriteLine($"Unify [{recursiveCount}-6]");

                var unified = fss1.
                    Select(fs => tss1.Any(ts => this.InternalUnify(ts, fs, overloadable, isWiden) != null)).
                    Memoize();

                var result = unified.All(w => w) ?
                    to :
                    null;

                Debug.WriteLine($"Unify [{recursiveCount--}-7]: Unified={unified}, Result={result}");

                return result;
            }

            // (int + double): (int + double) <-- int
            // (int + IServiceProvider): (int + IServiceProvider) <-- int
            // (int + IComparable): (int + IComparable) <-- string
            // (int + _): (int + _) <-- string
            // (int + _[1]): (int + _[1]) <-- _[2]
            if (to is ISumExpression(IExpression[] tss2))
            {
                Debug.WriteLine($"Unify [{recursiveCount}-8]");

                var unified = tss2.
                    Select(ts => this.InternalUnify(ts, from, overloadable, isWiden)).
                    Where(result => result != null).
                    Memoize();

                var result = OverloadTerm.From(unified!);

                Debug.WriteLine($"Unify [{recursiveCount--}-9]: Unified={unified}, Result={result}");

                return result;
            }

            // null: int <-- (int + double)
            if (from is ISumExpression(IExpression[] fss2))
            {
                Debug.WriteLine($"Unify [{recursiveCount}-10]");

                var unified = fss2.
                    Select(fs => this.InternalUnify(to, fs, overloadable, isWiden)).
                    Memoize();

                var result = unified.All(r => r != null) ?
                    OverloadTerm.From(unified!) :
                    null;

                Debug.WriteLine($"Unify [{recursiveCount--}-11]: Unified={unified}, Result={result}");

                return result;
            }

            // object: object <-- int
            // double: double <-- int
            // IComparable: IComparable <-- string
            if (to is ITypeTerm toType &&
                from is ITypeTerm fromType)
            {
                Debug.WriteLine($"Unify [{recursiveCount}-12]: ToType={toType}, FromType={fromType}");

                var result = toType.IsConvertibleFrom(fromType) ?
                    to :
                    null;

                Debug.WriteLine($"Unify [{recursiveCount--}-13]: Result={result}");

                return result;
            }

            if (to is IPlaceholderTerm tph)
            {
                var result = this.InternalUnifyPlaceholder(tph, from, true, overloadable, isWiden);

                Debug.WriteLine($"Unify [{recursiveCount--}-14]: Result={result}");

                return result;
            }
            if (from is IPlaceholderTerm fph)
            {
                var result = this.InternalUnifyPlaceholder(fph, to, false, overloadable, isWiden);

                Debug.WriteLine($"Unify [{recursiveCount--}-15]: Result={result}");

                return result;
            }

            var final = isWiden ?
                null :
                OverloadTerm.From(new[] { to, from });

            Debug.WriteLine($"Unify [{recursiveCount--}-16]: Final={final}");

            return final;
        }

        public void Unify(IExpression to, IExpression from) =>
            this.InternalUnify(to, from, true, false);
        public IExpression? Widen(IExpression to, IExpression from) =>
            this.InternalUnify(to, from, true, true);

        public IExpression? Resolve(IPlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (this.descriptions.TryGetValue(current.Index, out var combined))
                {
                    if (combined is IPlaceholderTerm p)
                    {
                        current = p;
                    }
                    else
                    {
                        return combined;
                    }
                }
                else
                {
                    return null;
                }
            }

        }

        public static InferContext Create(IRootTypeContext rootContext) =>
            new InferContext(rootContext);
    }
}
