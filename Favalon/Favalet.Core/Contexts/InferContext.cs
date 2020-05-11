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
        IPlaceholderTerm CreatePlaceholder();
        void Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        IExpression? Widen(IExpression to, IExpression from);
        IExpression? LookupPlaceholder(IPlaceholderTerm placeholder);
    }

    internal sealed class InferContext :
        TypeContext, IInferContext, IFixupContext
    {
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

        internal int DrawNextPlaceholderIndex() =>
            this.rootContext.DrawNextPlaceholderIndex();

        public IPlaceholderTerm CreatePlaceholder() =>
            PlaceholderTerm.Create(this, 1);

        private IExpression? Substitute(
            IPlaceholderTerm placeholder,
            IExpression from,
            bool isForward)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var lastCombined))
            {
                var result = isForward ?
                    this.Solve(lastCombined, from) :  // forward
                    this.Solve(from, lastCombined);   // backward

                if (result is IExpression)
                {
                    this.descriptions[placeholder.Index] = result;
                    return result;
                }
                else
                {
                    var combinedExpression = OverloadTerm.From(new[] { lastCombined, from });
                    this.descriptions[placeholder.Index] = combinedExpression!;
                    return null;
                }
            }
            else
            {
                this.descriptions.Add(placeholder.Index, from);
                return from;
            }
        }

        private IExpression? Solve(IExpression to, IExpression from)
        {
            // int: int <-- int
            // IComparable: IComparable <-- IComparable
            // _[1]: _[1] <-- _[1]
            if (to.ShallowEquals(from))
            {
                return to;
            }

            if (to is TerminationTerm || from is TerminationTerm)
            {
                return null;
            }

            if (to is UnspecifiedTerm || from is UnspecifiedTerm)
            {
                return null;
            }

            // int->object: int->object <-- object->int
            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.Solve(fp, tp);
                var rr = this.Solve(tr, fr);

                if (pr != null && rr != null)
                {
                    return FunctionDeclaredExpression.From(pr, rr);
                }
                else
                {
                    return null;
                }
            }

            // (int | double): (int | double) <-- (int | double)
            // (int | double | string): (int | double | string) <-- (int | double)
            // (int | IComparable): (int | IComparable) <-- (int | string)
            // null: (int | double) <-- (int | double | string)
            // null: (int | IServiceProvider) <-- (int | double)
            // (int | _): (int | _) <-- (int | string)
            // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])
            if (to is IOrExpression(IExpression[] tss1) &&
                from is IOrExpression(IExpression[] fss1))
            {
                var results = fss1.
                    Select(fs => tss1.Any(ts => this.Solve(ts, fs) != null)).
                    Memoize();
                return results.All(r => r) ?
                    to : null;
            }

            // (int | double): (int | double) <-- int
            // (int | IServiceProvider): (int | IServiceProvider) <-- int
            // (int | IComparable): (int | IComparable) <-- string
            // (int | _): (int | _) <-- string
            // (int | _[1]): (int | _[1]) <-- _[2]
            if (to is IOrExpression(IExpression[] tss2))
            {
                var results = tss2.
                    Select(ts => this.Solve(ts, from)).
                    Memoize();
                return results.Any(r => r != null) ?
                    OverloadTerm.From(results.Where(r => r != null)!) : null;
            }

            // null: int <-- (int | double)
            if (from is IOrExpression(IExpression[] fss2))
            {
                var results = fss2.
                    Select(fs => this.Solve(to, fs)).
                    Memoize();
                return results.All(r => r != null) ?
                    OverloadTerm.From(results!) : null;
            }

            // object: object <-- int
            // double: double <-- int
            // IComparable: IComparable <-- string
            if (to is ITypeTerm toType &&
                from is ITypeTerm fromType)
            {
                return toType.IsConvertibleFrom(fromType) ?
                    to :
                    null;
            }

            if (to is IPlaceholderTerm tph)
            {
                return this.Substitute(tph, from, true);
            }
            if (from is IPlaceholderTerm fph)
            {
                return this.Substitute(fph, to, false);
            }

            return null;
        }

        public void Unify(IExpression to, IExpression from) =>
            this.Solve(to, from);

        public IExpression? Widen(IExpression to, IExpression from) =>
            this.Solve(to, from);

        public IExpression? LookupPlaceholder(IPlaceholderTerm placeholder)
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
