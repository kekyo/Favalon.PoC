using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections;
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
        
#if DEBUG
        private sealed class PlaceholderMarker
        {
            private readonly HashSet<string> symbols;
            private readonly List<string> list;

            private PlaceholderMarker(HashSet<string> symbols, List<string> list)
            {
                this.symbols = symbols;
                this.list = list;
            }

            public bool Mark(string targetSymbol)
            {
                list.Add(targetSymbol);
                return symbols.Add(targetSymbol);
            }

            public PlaceholderMarker Fork() =>
                new PlaceholderMarker(new HashSet<string>(this.symbols), new List<string>(this.list));

            public override string ToString() =>
                StringUtilities.Join(" --> ", this.list);
            
            public static PlaceholderMarker Create() =>
                new PlaceholderMarker(new HashSet<string>(), new List<string>());
        }

        private void Verify(PlaceholderMarker marker, IExpression expression)
        {
            if (expression is IIdentityTerm identity)
            {
                this.Verify(marker, identity.Symbol);
            }
            else if (expression is IFunctionExpression(IExpression p, IExpression r))
            {
                this.Verify(marker.Fork(), p);
                this.Verify(marker.Fork(), r);
            }
        }
        
        private void Verify(PlaceholderMarker marker, string symbol)
        {
            var targetSymbol = symbol;
            while (true)
            {
                if (marker.Mark(targetSymbol))
                {
                    if (this.unifications.TryGetValue(targetSymbol, out var resolved))
                    {
                        if (resolved is IIdentityTerm identity)
                        {
                            targetSymbol = identity.Symbol;
                            continue;
                        }
                        else
                        {
                            this.Verify(marker, resolved);
                        }
                    }
                    return;
                }

                throw new InvalidOperationException(
                    "Detected circular variable reference: " + marker);
            }
        }
#endif
            
        private void Update(string symbol, IExpression expression)
        {
#if DEBUG
            if (this.unifications.TryGetValue(symbol, out var origin))
            {
                if (!origin.Equals(expression))
                {
                    Debug.WriteLine(
                        $"Unifier.Update: {symbol}: {origin.GetPrettyString(PrettyStringTypes.Readable)} ==> {expression.GetPrettyString(PrettyStringTypes.Readable)}");
                }
            }
#endif
            this.unifications[symbol] = expression;
#if DEBUG
            this.Verify(PlaceholderMarker.Create(), symbol);
#endif
        }

        public void RegisterPair(IIdentityTerm identity, IExpression expression) =>
            this.Update(identity.Symbol, expression);

        private IExpression InternalUnifyBothPlaceholders(
            IReduceContext context, IIdentityTerm from, IIdentityTerm to)
        {
            // Greater prioritize by exist unification rather than not exist.
            // Because will check ignoring circular reference at recursive path [1].
            switch
                (this.unifications.TryGetValue(from.Symbol, out var rfrom),
                 this.unifications.TryGetValue(to.Symbol, out var rto))
            {
                case (true, false):
                    var result1 = this.InternalUnify(context, rfrom, to);
                    this.Update(from.Symbol, result1);
                    return from;
                case (false, true):
                    var result2 = this.InternalUnify(context, from, rto);
                    this.Update(to.Symbol, result2);
                    return to;
            }
            
            switch (from, to)
            {
                case (PlaceholderTerm _, _):
                    this.Update(from.Symbol, to);
                    return from;
                default:
                    this.Update(to.Symbol, from);
                    return to;
            }
        }

        private IExpression InternalUnifyPlaceholder(
            IReduceContext context, IIdentityTerm from, IExpression to)
        {
            if (this.unifications.TryGetValue(from.Symbol, out var target))
            {
                var result = this.InternalUnify(context, to, target);
                this.Update(from.Symbol, result);
            }
            else
            {
                this.Update(from.Symbol, to);
            }
            
            return from;
        }

        private IExpression InternalUnifyCore(
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
                var parameter = this.InternalUnify(context, fp, tp);
                var result = this.InternalUnify(context, fr, tr);

                var function = FunctionExpression.Create(
                    parameter, result, context, PlaceholderOrderHints.TypeOrAbove);
                return function;
            }

            var combined = OrExpression.Create(from, to);
            var calculated = context.TypeCalculator.Compute(combined);

            var rewritable = context.MakeRewritable(calculated);
            var inferred = context.Infer(rewritable);

            return inferred;
        }

        private IExpression InternalUnify(
            IReduceContext context, IExpression from, IExpression to)
        {
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
                    this.InternalUnify(context, from.HigherOrder, to.HigherOrder);

                    // Unification.
                    return this.InternalUnifyCore(context, from, to);
            }
        }

        public void Unify(
            IReduceContext context, IExpression from, IExpression to) =>
            this.InternalUnify(context, from, to);

        public IExpression? Resolve(string symbol)
        {
#if DEBUG
            this.Verify(PlaceholderMarker.Create(), symbol);
#endif
            return this.unifications.TryGetValue(symbol, out var resolved) ?
                resolved :
                null;
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
