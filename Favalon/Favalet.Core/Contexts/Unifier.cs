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

        public Unifier()
        {
        }

        private void Update(string symbol, IExpression expression)
        {
#if DEBUG
            if (this.unifications.TryGetValue(symbol, out var origin) &&
                !origin.Equals(expression))
            {
                Debug.WriteLine(
                    $"Unifier.Update: {symbol}: {origin.GetPrettyString(PrettyStringTypes.Readable)} ==> {expression.GetPrettyString(PrettyStringTypes.Readable)}");
            }
#endif
            this.unifications[symbol] = expression;
        }

        public void RegisterPair(IIdentityTerm identity, IExpression expression) =>
            this.Update(identity.Symbol, expression);

        private void InternalUnifyBothPlaceholders(
            IReduceContext context, IIdentityTerm from, IIdentityTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            if (this.unifications.TryGetValue(from.Symbol, out var rfrom))
            {
                this.Unify(context, rfrom, to);
            }
            else if (this.unifications.TryGetValue(to.Symbol, out var rto))
            {
                this.Unify(context, from, rto);
            }
            else if (from is PlaceholderTerm)
            {
                this.Update(from.Symbol, to);
            }
            else
            {
                this.Update(to.Symbol, from);
            }
        }

        private void InternalUnifyPlaceholder(
            IReduceContext context, IIdentityTerm from, IExpression to)
        {
            if (this.unifications.TryGetValue(from.Symbol, out var target))
            {
                this.Unify(context, to, target);
            }
            else
            {
                this.Update(from.Symbol, to);
            }
        }

        private void InternalUnifyCore(
            IReduceContext context, IExpression from, IExpression to)
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
                        return;
                    }
                    else
                    {
                        // Unify both placeholders.
                        this.InternalUnifyBothPlaceholders(context, fi, ti);
                        return;
                    }
                }
                else
                {
                    // Unify from placeholder.
                    this.InternalUnifyPlaceholder(context, fi, to);
                    return;
                }
            }
            else if (to is IIdentityTerm ti)
            {
                // Unify to placeholder.
                this.InternalUnifyPlaceholder(context, ti, from);
                return;
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                this.Unify(context, fp, tp);
                this.Unify(context, fr, tr);
                return;
            }

            if (context.TypeCalculator.Equals(from, to))
            {
                return;
            }

            // Can't accept from --> to
            throw new ArgumentException(
                $"Couldn't accept unification: From=\"{from.GetPrettyString(PrettyStringTypes.StrictAll)}\", To=\"{to.GetPrettyString(PrettyStringTypes.StrictAll)}\".");
        }

        public void Unify(
            IReduceContext context, IExpression from, IExpression to)
        {
            if (object.ReferenceEquals(from, to))
            {
                return;
            }

            switch (from, to)
            {
                // Ignore DeadEndTerm unification.
                case (DeadEndTerm _, _):
                case (_, DeadEndTerm _):
                    break;

                default:
                    // Unification.
                    this.InternalUnifyCore(context, from, to);

                    // Unification higher order.
                    this.Unify(context, from.HigherOrder, to.HigherOrder);
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
