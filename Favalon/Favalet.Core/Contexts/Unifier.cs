using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts
{
    internal sealed class Unifier
    {
        private readonly ILogicalCalculator typeCalculator;
        private readonly Dictionary<int, IExpression> unifiedExpressions =
            new Dictionary<int, IExpression>();

        public Unifier(ILogicalCalculator typeCalculator) =>
            this.typeCalculator = typeCalculator;

        private void UnifyPlaceholder(int fromIndex, IExpression to)
        {
            // TODO: variance
            lock (this.unifiedExpressions)
            {
                if (this.unifiedExpressions.TryGetValue(fromIndex, out var target))
                {
                    this.Unify(to, target);
                }
                else
                {
                    this.unifiedExpressions[fromIndex] = to;
                }
            }
        }

        private void UnifyCore(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            if (from is IPlaceholderTerm(int fromIndex))
            {
                if (to is IPlaceholderTerm(int toIndex) && (fromIndex == toIndex))
                {
                    return;
                }
                else
                {
                    this.UnifyPlaceholder(fromIndex, to);
                    return;
                }
            }
            else if (to is IPlaceholderTerm(int toIndex))
            {
                this.UnifyPlaceholder(toIndex, from);
                return;
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                this.Unify(fp, tp);
                this.Unify(fr, tr);
                return;
            }

            if (this.typeCalculator.Equals(from, to))
            {
                return;
            }

            // Can't accept from --> to
            throw new ArgumentException(
                $"Couldn't accept unification: From=\"{from.GetPrettyString(PrettyStringContext.Strict)}\", To=\"{to.GetPrettyString(PrettyStringContext.Strict)}\".");
        }

        public void Unify(IExpression from, IExpression to)
        {
            this.UnifyCore(from, to);

            switch (from, to)
            {
                case (FourthTerm _, _):
                case (_, FourthTerm _):
                    break;

                default:
                    this.Unify(from.HigherOrder, to.HigherOrder);
                    break;
            }
        }

        private sealed class PlaceholderMarker
        {
            private readonly HashSet<int> indexes = new HashSet<int>();
            private readonly List<int> list = new List<int>();

            public bool Mark(int placeholderIndex)
            {
                list.Add(placeholderIndex);
                return indexes.Add(placeholderIndex);
            }

            public override string ToString() =>
                StringUtilities.Join(" --> ", this.list.Select(index => $"'{index}"));
        }

        public IExpression? ResolvePlaceholderIndex(int index)
        {
            var marker = new PlaceholderMarker();
            var targetIndex = index;
            IExpression? lastExpression = null;

            while (true)
            {
                if (marker.Mark(targetIndex))
                {
                    if (this.unifiedExpressions.TryGetValue(targetIndex, out var resolved))
                    {
                        lastExpression = resolved;
                        if (lastExpression is IPlaceholderTerm placeholder)
                        {
                            targetIndex = placeholder.Index;
                            continue;
                        }
                    }

                    return lastExpression;
                }

                throw new InvalidOperationException(
                    "Detected circular variable reference: " + marker);
            }
        }
    }
}
