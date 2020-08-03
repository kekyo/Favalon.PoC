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
        private readonly IReduceContext context;
        private readonly Dictionary<string, IExpression> unifications =
            new Dictionary<string, IExpression>();

        public Unifier(IReduceContext context) =>
            this.context = context;

        private IExpression InternalUnifyBothPlaceholders(IIdentityTerm from, IIdentityTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            if (this.unifications.TryGetValue(from.Symbol, out var rfrom))
            {
                return this.Unify(rfrom, to);
            }
            else if (this.unifications.TryGetValue(to.Symbol, out var rto))
            {
                return this.Unify(from, rto);
            }
            else if (from is PlaceholderTerm)
            {
                this.unifications[from.Symbol] = to;
                return to;
            }
            else
            {
                this.unifications[to.Symbol] = from;
                return from;
            }
        }

        private IExpression InternalUnifyPlaceholder(IIdentityTerm from, IExpression to)
        {
            if (this.unifications.TryGetValue(from.Symbol, out var target))
            {
                return this.Unify(to, target);
            }
            else
            {
                this.unifications[from.Symbol] = to;
                return to;
            }
        }

        private IExpression InternalUnifyCore(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            // Interpret placeholders.
            if (from is IIdentityTerm(string fromSymbol) fi)
            {
                if (to is IIdentityTerm(string toSymbol) ti)
                {
                    // [1]
                    if (fromSymbol == toSymbol)
                    {
                        // Ignore equal placeholders.
                        return to;
                    }
                    else
                    {
                        // Unify both placeholders.
                        return this.InternalUnifyBothPlaceholders(fi, ti);
                    }
                }
                else
                {
                    // Unify from placeholder.
                    return this.InternalUnifyPlaceholder(fi, to);
                }
            }
            else if (to is IIdentityTerm ti)
            {
                // Unify to placeholder.
                return this.InternalUnifyPlaceholder(ti, from);
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var parameter = this.Unify(fp, tp);
                var result = this.Unify(fr, tr);

                if (object.ReferenceEquals(fp, parameter) &&
                    object.ReferenceEquals(fr, result))
                {
                    return from;
                }
                else if (object.ReferenceEquals(tp, parameter) &&
                    object.ReferenceEquals(tr, result))
                {
                    return to;
                }
                else
                {
                    return FunctionExpression.Create(
                        parameter,
                        result,
                        this.context,
                        PlaceholderOrderHints.TypeOrAbove);
                }
            }

            // Logical calculation
            var widening = OrExpression.Create(from, to);
            return this.context.TypeCalculator.Compute(widening);
        }

        public IExpression Unify(IExpression from, IExpression to)
        {
            if (object.ReferenceEquals(from, to))
            {
                return to;
            }

            switch (from, to)
            {
                // Ignore DeadEndTerm unification.
                case (DeadEndTerm _, _):
                case (_, DeadEndTerm _):
                    return DeadEndTerm.Instance;

                default:
                    // Unification higher order.
                    var unifiedHigherOrder = this.Unify(from.HigherOrder, to.HigherOrder);

                    // Unification.
                    var unified = this.InternalUnifyCore(from, to);
                    break;
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
            // Release build code may cause stack overflow by recursive Fixup() calls
            // and the debugger will be crashed,
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
