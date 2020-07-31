using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Favalet.Contexts
{
    [DebuggerDisplay("{Simple}")]
    internal sealed class Unifier
    {
        private readonly ILogicalCalculator typeCalculator;
        private readonly Dictionary<int, IExpression> unifications =
            new Dictionary<int, IExpression>();

        public Unifier(ILogicalCalculator typeCalculator) =>
            this.typeCalculator = typeCalculator;

        private void InternalUnifyBothPlaceholders(IPlaceholderTerm from, IPlaceholderTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            if (this.unifications.TryGetValue(from.Index, out var rfrom))
            {
                this.InternalUnify(rfrom, to);
            }
            else if (this.unifications.TryGetValue(to.Index, out var rto))
            {
                this.InternalUnify(from, rto);
            }
            else
            {
                this.unifications[from.Index] = to;
#if DEBUG
                // DEBUG: Check invalid circular reference.
                ResolvePlaceholderIndex(from.Index);
#endif
            }
        }

        private void InternalUnifyPlaceholder(int fromIndex, IExpression to)
        {
            if (this.unifications.TryGetValue(fromIndex, out var target))
            {
                this.InternalUnify(to, target);
            }
            else
            {
                this.unifications[fromIndex] = to;
#if DEBUG
                // DEBUG: Check invalid circular reference.
                ResolvePlaceholderIndex(fromIndex);
#endif
            }
        }

        private void InternalUnifyCore(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            // Interpret placeholders.
            if (from is IPlaceholderTerm(int fromIndex) fph)
            {
                if (to is IPlaceholderTerm(int toIndex) tph)
                {
                    // [1]
                    if (fromIndex == toIndex)
                    {
                        // Ignore equal placeholders.
                        return;
                    }
                    else
                    {
                        // Unify both placeholders.
                        this.InternalUnifyBothPlaceholders(fph, tph);
                        return;
                    }
                }
                else
                {
                    // Unify from placeholder.
                    this.InternalUnifyPlaceholder(fromIndex, to);
                    return;
                }
            }
            else if (to is IPlaceholderTerm(int toIndex))
            {
                // Unify to placeholder.
                this.InternalUnifyPlaceholder(toIndex, from);
                return;
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                this.InternalUnify(fp, tp);
                this.InternalUnify(fr, tr);
                return;
            }

            // Logical equals.
            if (this.typeCalculator.Equals(from, to))
            {
                return;
            }

            // Can't accept from --> to
            throw new ArgumentException(
                $"Couldn't accept unification: From=\"{from.GetPrettyString(PrettyStringTypes.StrictAll)}\", To=\"{to.GetPrettyString(PrettyStringTypes.StrictAll)}\".");
        }

        private void InternalUnify(IExpression from, IExpression to)
        {
            switch (from, to)
            {
                // Ignore TerminationTerm unification.
                case (TerminationTerm _, _):
                case (_, TerminationTerm _):
                    break;

                default:
                    // Unification.
                    this.InternalUnifyCore(from, to);

                    // Unification higher order.
                    this.InternalUnify(from.HigherOrder, to.HigherOrder);
                    break;
            }
        }

        public void Unify(IExpression from, IExpression to)
        {
            lock (this.unifications)
            {
                this.InternalUnify(from, to);
            }
        }

#if DEBUG
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
#endif

        public IExpression? ResolvePlaceholderIndex(int index)
        {
#if DEBUG
            // Release build code may cause stack overflow by recursive Fixup() calls,
            // so it's dodging by the loop (only applicable nested placeholders.)
            var marker = new PlaceholderMarker();
            var targetIndex = index;
            IExpression? lastExpression = null;

            while (true)
            {
                if (marker.Mark(targetIndex))
                {
                    if (this.unifications.TryGetValue(targetIndex, out var resolved))
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
#else
            return this.unifications.TryGetValue(index, out var resolved) ?
                resolved :
                null;
#endif
        }

        public string Xml =>
            new XElement(
                "Unifier",
                this.unifications.
                OrderBy(entry => entry.Key).
                Select(entry => new XElement("Unification",
                    new XAttribute("index", entry.Key),
                    entry.Value.GetXml())).
                Memoize()).
            ToString();

        public string Simple =>
            StringUtilities.Join(
                Environment.NewLine,
                this.unifications.
                OrderBy(entry => entry.Key).
                Select(entry => $"'{entry.Key} --> {entry.Value.GetPrettyString(PrettyStringTypes.Readable)}"));

        public override string ToString() =>
            "Unifier: " + this.Simple;
    }
}
