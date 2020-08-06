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
        private readonly Dictionary<string, IExpression> unifications =
            new Dictionary<string, IExpression>();

        private Unifier()
        {
        }

        private IExpression InternalUnifyBothPlaceholders(
            IReduceContext context,
            IIdentityTerm from,
            IIdentityTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            if (this.unifications.TryGetValue(from.Symbol, out var rfrom))
            {
                return this.InternalUnify(context, rfrom, to);
            }
            else if (this.unifications.TryGetValue(to.Symbol, out var rto))
            {
                return this.InternalUnify(context, from, rto);
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

        private IExpression InternalUnifyPlaceholder(
            IReduceContext context,
            IIdentityTerm from,
            IExpression to)
        {
            if (this.unifications.TryGetValue(from.Symbol, out var target))
            {
                return this.InternalUnify(context, to, target);
            }
            else
            {
                this.unifications[from.Symbol] = to;
                return to;
            }
        }

        private IExpression InternalUnifyCore(
            IReduceContext context,
            IExpression from,
            IExpression to)
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
                        return from;
                    }
                    else
                    {
                        // Unify both placeholders.
                        return this.InternalUnifyBothPlaceholders(context, fi, ti);
                    }
                }
                else
                {
                    // Unify from placeholder.
                    return this.InternalUnifyPlaceholder(context, fi, to);
                }
            }
            else if (to is IIdentityTerm ti)
            {
                // Unify to placeholder.
                return this.InternalUnifyPlaceholder(context, ti, from);
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var f = this.InternalUnify(context, fp, tp);
                var r = this.InternalUnify(context, fr, tr);
                return FunctionExpression.Create(
                    f,
                    r,
                    context,
                    PlaceholderOrderHints.TypeOrAbove);
            }

            // Logical equals.
            if (context.TypeCalculator.Equals(from, to))
            {
                return from;
            }
            
            // 
            var placeholder =
                context.CreatePlaceholder(PlaceholderOrderHints.TypeOrAbove);

            var combined = OrExpression.Create(from, to);
            var calculated = context.TypeCalculator.Compute(combined);

            var result = this.InternalUnify(context, placeholder, calculated);

            return result;
        }

        private IExpression InternalUnify(
            IReduceContext context,
            IExpression from,
            IExpression to)
        {
            // Make short circuit.
            if (object.ReferenceEquals(from, to))
            {
                return from;
            }

            switch (from, to)
            {
                // Ignore DeadEndTerm unification.
                case (DeadEndTerm _, _):
                case (_, DeadEndTerm _):
                    return DeadEndTerm.Instance;

                default:
                    // Unification higher order.
                    var higherOrder = this.InternalUnify(context, from.HigherOrder, to.HigherOrder);

                    // Unification.
                    return this.InternalUnifyCore(context, from, to);
            }
        }

        public void Unify(
            IReduceContext context,
            IExpression from,
            IExpression to) =>
            this.InternalUnify(context, from, to);

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
        
        public static Unifier Create() =>
            new Unifier();
    }
}
