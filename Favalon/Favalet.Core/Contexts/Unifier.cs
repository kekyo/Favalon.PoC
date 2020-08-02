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
        private readonly Dictionary<string, IExpression> unifications =
            new Dictionary<string, IExpression>();

        public Unifier(ILogicalCalculator typeCalculator) =>
            this.typeCalculator = typeCalculator;

        private void InternalUnifyBothPlaceholders(IIdentityTerm from, IIdentityTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            if (this.unifications.TryGetValue(from.Symbol, out var rfrom))
            {
                this.InternalUnify(rfrom, to);
            }
            else if (this.unifications.TryGetValue(to.Symbol, out var rto))
            {
                this.InternalUnify(from, rto);
            }
            else if (from is PlaceholderTerm)
            {
                Debug.Assert(to.Symbol != "'8");
                this.unifications[from.Symbol] = to;
            }
            else
            {
                Debug.Assert(from.Symbol != "'8");
                this.unifications[to.Symbol] = from;
            }
        }

        private void InternalUnifyPlaceholder(IIdentityTerm from, IExpression to)
        {
            if (this.unifications.TryGetValue(from.Symbol, out var target))
            {
                this.InternalUnify(to, target);
            }
            else
            {
                this.unifications[from.Symbol] = to;
            }
        }

        private void InternalUnifyCore(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            // Interpret placeholders.
            if (from is IIdentityTerm(string fromSymbol) fph)
            {
                if (to is IIdentityTerm(string toSymbol) tph)
                {
                    // [1]
                    if (fromSymbol == toSymbol)
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
                    this.InternalUnifyPlaceholder(fph, to);
                    return;
                }
            }
            else if (to is IIdentityTerm tph)
            {
                // Unify to placeholder.
                this.InternalUnifyPlaceholder(tph, from);
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
            if (object.ReferenceEquals(from, to))
            {
                return;
            }

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
            if (object.ReferenceEquals(from, to))
            {
                return;
            }

            lock (this.unifications)
            {
                this.InternalUnify(from, to);
            }
        }

#if DEBUG
        private sealed class PlaceholderMarker
        {
            private readonly HashSet<string> symbols = new HashSet<string>();
            private readonly List<string> list = new List<string>();

            public bool Mark(string targetSymbol)
            {
                list.Add(targetSymbol);
                return symbols.Add(targetSymbol);
            }

            public override string ToString() =>
                StringUtilities.Join(" --> ", this.list);
        }
#endif

        public IExpression? Resolve(string symbol)
        {
#if DEBUG
            // Release build code may cause stack overflow by recursive Fixup() calls,
            // so it's dodging by the loop (only applicable nested placeholders.)
            var marker = new PlaceholderMarker();
            var targetSymbol = symbol;
            IExpression? lastExpression = null;

            while (true)
            {
                if (marker.Mark(targetSymbol))
                {
                    if (this.unifications.TryGetValue(targetSymbol, out var resolved))
                    {
                        lastExpression = resolved;
                        if (lastExpression is IIdentityTerm identity)
                        {
                            targetSymbol = identity.Symbol;
                            continue;
                        }
                    }

                    return lastExpression;
                }

                throw new InvalidOperationException(
                    "Detected circular variable reference: " + marker);
            }
#else
            return this.unifications.TryGetValue(symbol, out var resolved) ?
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
                    new XAttribute("symbol", entry.Key),
                    entry.Value.GetXml())).
                Memoize()).
            ToString();

        public string Simple =>
            StringUtilities.Join(
                Environment.NewLine,
                this.unifications.
                OrderBy(entry => entry.Key).
                Select(entry => $"{entry.Key} --> {entry.Value.GetPrettyString(PrettyStringTypes.Readable)}"));

        public override string ToString() =>
            "Unifier: " + this.Simple;
    }
}
