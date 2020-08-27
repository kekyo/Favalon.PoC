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
    internal enum UnifyConstraints
    {
        Free,
        Fixed
    }
    
    [DebuggerDisplay("{Simple}")]
    internal sealed class Unifier :
        FixupContext,  // Because used by "Simple" property implementation.
        IUnsafePlaceholderResolver
    {
        [DebuggerStepThrough]
        private readonly struct Unification
        {
            public readonly IExpression Expression;
            public readonly bool Fixed;

            private Unification(IExpression expression, bool @fixed)
            {
                this.Expression = expression;
                this.Fixed = @fixed;
            }

            public string ToString(PrettyStringTypes type)
            {
                var @fixed = this.Fixed ? "Fixed," : string.Empty;
                return $"{@fixed}{this.Expression.GetPrettyString(type)}";
            }
            public override string ToString() =>
                this.ToString(PrettyStringTypes.Readable);

            public static Unification Create(IExpression expression, bool @fixed) =>
                new Unification(expression, @fixed);
        }
        
        private readonly Dictionary<int, Unification> unifications =
            new Dictionary<int, Unification>();
        
        [DebuggerStepThrough]
        private Unifier(ITypeCalculator typeCalculator) :
            base(typeCalculator)
        {
        }

        [DebuggerStepThrough]
        private sealed class PlaceholderMarker
        {
            private readonly HashSet<int> indexes;
#if DEBUG
            private readonly List<int> list;
#endif
            private PlaceholderMarker(
#if DEBUG
                HashSet<int> indexes, List<int> list
#else
                HashSet<int> indexes
#endif
            )
            {
                this.indexes = indexes;
#if DEBUG
                this.list = list;
#endif
            }

            public bool Mark(int targetIndex)
            {
#if DEBUG
                list.Add(targetIndex);
#endif
                return indexes.Add(targetIndex);
            }

            public PlaceholderMarker Fork() =>
#if DEBUG
                new PlaceholderMarker(new HashSet<int>(this.indexes), new List<int>(this.list));
#else
                new PlaceholderMarker(new HashSet<int>(this.symbols));
#endif

#if DEBUG
            public override string ToString() =>
                StringUtilities.Join(" ==> ", this.list.Select(index => $"'{index}"));
#endif

            public static PlaceholderMarker Create() =>
#if DEBUG
                new PlaceholderMarker(new HashSet<int>(), new List<int>());
#else
                new PlaceholderMarker(new HashSet<int>());
#endif
        }

        private void Occur(PlaceholderMarker marker, IExpression expression)
        {
            if (expression is IPlaceholderTerm identity)
            {
                this.Occur(marker, identity.Index);
            }
            else if (expression is IFunctionExpression(IExpression p, IExpression r))
            {
                this.Occur(marker.Fork(), p);
                this.Occur(marker.Fork(), r);
            }
        }

        private void Occur(PlaceholderMarker marker, int index)
        {
            var targetIndex = index;
            while (true)
            {
                if (marker.Mark(targetIndex))
                {
                    if (this.unifications.TryGetValue(targetIndex, out var resolved))
                    {
                        if (resolved.Expression is IPlaceholderTerm placeholder)
                        {
                            targetIndex = placeholder.Index;
                            continue;
                        }
                        else
                        {
                            this.Occur(marker, resolved.Expression);
                        }
                    }

                    return;
                }
#if DEBUG
                Debug.WriteLine(
                    "Detected circular variable reference: " + marker);
                throw new InvalidOperationException(
                    "Detected circular variable reference: " + marker);
#else
                throw new InvalidOperationException(
                    "Detected circular variable reference: " + symbol);
#endif
            }
        }

        private void Update(int index, IExpression expression, bool @fixed)
        {
            if (this.unifications.TryGetValue(index, out var lastUnification))
            {
                var unification = Unification.Create(
                    expression,
                    lastUnification.Fixed || @fixed);     // Derived if already fixed.
                
                if (!lastUnification.Equals(unification))
                {
                    this.unifications[index] = unification;

                    Debug.WriteLine(
                        $"Unifier.Update: '{index}: {lastUnification} ==> {unification}");

                    this.Occur(PlaceholderMarker.Create(), index);
                }
            }
            else
            {
                var unification = Unification.Create(expression, @fixed);
                this.unifications.Add(index, unification);

                Debug.WriteLine(
                    $"Unifier.Update: '{index}: {unification}");

                this.Occur(PlaceholderMarker.Create(), index);
            }
        }

        [DebuggerStepThrough]
        private readonly struct Attribute
        {
            public readonly bool Forward;
            public readonly bool Fixed;

            private Attribute(bool forward, bool @fixed)
            {
                this.Forward = forward;
                this.Fixed = @fixed;
            }

            public Attribute Reverse() =>
                new Attribute(!this.Forward, this.Fixed);
            
            public Attribute ApplyFixed(bool @fixed) =>
                new Attribute(this.Forward, this.Fixed || @fixed);

            public override string ToString() =>
                (this.Forward, this.Fixed) switch
                {
                    (false, false) => "Backward",
                    (false, true) => "Backward,Fixed",
                    (true, false) => "Forward",
                    (true, true) => "Forward,Fixed",
                };

            public static Attribute Create(bool @fixed) =>
                new Attribute(true, @fixed);
        }

        private void InternalUnifyPlaceholder(
            IInferContext context,
            IPlaceholderTerm from,
            IExpression to,
            Unification examinedFrom,
            Attribute attribute)
        {
            // (origin, current)
            switch (examinedFrom.Fixed, attribute.Fixed)
            {
                case (true, true):
                    // Cannot update `from.Index`, will check only compatibility.
                    if (this.InternalUnify(
                        context, examinedFrom.Expression, to, attribute) is IExpression result0)
                    {
                        var rresult = this.UnsafeResolveWhile(result0);
                        var tresult = this.UnsafeResolveWhile(to);
                        
                        var combined = attribute.Forward ?
                            (IExpression)OrExpression.Create(tresult, rresult) :  // Covariance.
                            AndExpression.Create(tresult, rresult);               // Contravariance.
                        var calculated = context.TypeCalculator.Compute(combined);

                        if (!context.TypeCalculator.Equals(calculated, rresult))
                        {
                            throw new InvalidOperationException(
                                $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
                        }
                    }
                    break;
                
                case (false, true):
                    // Force update.
                    this.Update(from.Index, to, attribute.Fixed);
                
                    // Must reinterprets examinedFrom.
                    if (this.InternalUnify(
                        context, examinedFrom.Expression, to, attribute) is IExpression result1)
                    {
                        var rresult = this.UnsafeResolveWhile(result1);
                        var tresult = this.UnsafeResolveWhile(to);
                    
                        var combined = attribute.Forward ?
                            (IExpression)OrExpression.Create(tresult, rresult) :  // Covariance.
                            AndExpression.Create(tresult, rresult);               // Contravariance.
                        var calculated = context.TypeCalculator.Compute(combined);

                        if (!context.TypeCalculator.Equals(calculated, rresult))
                        {
                            throw new InvalidOperationException(
                                $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
                        }
                    }
                    break;
                
                default:
                    // Derived fixed attribute.
                    var rattribute = Attribute.Create(examinedFrom.Fixed || attribute.Fixed);
                    
                    if (this.InternalUnify(
                        context, examinedFrom.Expression, to, rattribute) is IExpression result2)
                    {
                        if (!examinedFrom.Fixed)
                        {
                            this.Update(from.Index, result2, rattribute.Fixed);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
                        }
                    }
                    break;
            }
        }

        private void InternalUnifyBothPlaceholders(
            IInferContext context,
            IPlaceholderTerm from,
            IPlaceholderTerm to,
            Attribute attribute)
        {
            var rf = this.unifications.TryGetValue(from.Index, out var rfrom);
            var rt = this.unifications.TryGetValue(to.Index, out var rto);
            
            switch (rf, rt)
            {
                case (true, true):
                    var rattribute = Attribute.Create(rfrom.Fixed || rto.Fixed || attribute.Fixed);     // Derived if already fixed.
                    if (this.InternalUnify(
                        context,
                        rfrom.Expression,
                        rto.Expression,
                        rattribute) is IExpression result0)
                    {
                        this.Update(from.Index, result0, rattribute.Fixed);
                        this.Update(to.Index, result0, rattribute.Fixed);
                    }
                    break;
                
                case (true, false):
                    this.InternalUnifyPlaceholder(
                        context, from, to, rfrom, attribute);
                    break;
                
                case (false, true):
                    this.InternalUnifyPlaceholder(
                        context, to, from, rto, attribute.Reverse());    // Reversed order.
                    break;
                
                default:
                    this.Update(from.Index, to, attribute.Fixed);
                    break;
            }
        }

        private void InternalUnifyPlaceholder(
            IInferContext context,
            IPlaceholderTerm from,
            IExpression to,
            Attribute attribute)
        {
            if (this.unifications.TryGetValue(from.Index, out var rfrom))
            {
                this.InternalUnifyPlaceholder(
                    context, from, to, rfrom, attribute);
            }
            else
            {
                this.Update(from.Index, to, attribute.Fixed);
            }
        }

        private IExpression? InternalUnifyCore(
            IInferContext context,
            IExpression from,
            IExpression to,
            Attribute attribute)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            // Interpret placeholders.
            if (from is IPlaceholderTerm(int fromIndex) fph)
            {
                if (to is IPlaceholderTerm(int toIndex) tph)
                {
                    // [1]
                    if (fromIndex != toIndex)
                    {
                        // Unify both placeholders.
                        this.InternalUnifyBothPlaceholders(
                            context, fph, tph, attribute);
                    }
                }
                else
                {
                    // Unify from placeholder.
                    this.InternalUnifyPlaceholder(
                        context, fph, to, attribute);
                }
                return null;
            }
            else if (to is IPlaceholderTerm tph)
            {
                // Unify to placeholder.
                this.InternalUnifyPlaceholder(
                    context, tph, from, attribute.Reverse());    // Reversed order.
                return null;
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var parameter = this.InternalUnify(
                    context, fp, tp, attribute);
                var result = this.InternalUnify(
                    context, fr, tr, attribute);

                if (parameter is IExpression || result is IExpression)
                {
                    return FunctionExpression.Create(
                        parameter is IExpression ? parameter : fp,
                        result is IExpression ? result : fr);
                }
                else
                {
                    return null;
                }
            }

            var combined = attribute.Forward ?
                (IExpression)OrExpression.Create(from, to) :  // Covariance.
                AndExpression.Create(from, to);               // Contravariance.
            var calculated = context.TypeCalculator.Compute(combined);

            var rewritable = context.MakeRewritable(calculated);
            var inferred = context.Infer(rewritable);

            return inferred;
        }

        private IExpression? InternalUnify(
            IInferContext context,
            IExpression from,
            IExpression to,
            Attribute attribute)
        {
            // Same as.
            if (object.ReferenceEquals(from, to))
            {
                return null;
            }

            switch (from, to)
            {
                // Ignore IIgnoreUnificationTerm unification.
                case (IIgnoreUnificationTerm _, _):
                case (_, IIgnoreUnificationTerm _):
                    return null;

                default:
                    // Unify higher order (ignored result.)
                    this.InternalUnify(
                        context, from.HigherOrder, to.HigherOrder, attribute);

                    // Unify.
                    return this.InternalUnifyCore(
                        context, from, to, attribute);
            }
        }

        [DebuggerStepThrough]
        public void Unify(
            IInferContext context,
            IExpression from,
            IExpression to,
            bool @fixed) =>
            this.InternalUnify(
                context, from, to, Attribute.Create(@fixed));

        public override IExpression? Resolve(int index)
        {
#if DEBUG
            this.Occur(PlaceholderMarker.Create(), index);
#endif
            return this.unifications.TryGetValue(index, out var resolved) ?
                resolved.Expression :
                null;
        }

        public string Xml =>
            new XElement(
                "Unifier",
                this.unifications.OrderBy(entry => entry.Key).Select(entry => new XElement("Unification",
                    new XAttribute("symbol", entry.Key),
                    entry.Value.Expression.GetXml())).Memoize()).ToString();
        
        public string Simple =>
            StringUtilities.Join(
                Environment.NewLine,
                this.unifications.
                OrderBy(entry => entry.Key).
                Select(entry => string.Format(
                    "'{0} ==> {1}{2}",
                    entry.Key,
                    entry.Value.ToString(PrettyStringTypes.Minimum),
                    // It will not resolve any function types.
                    this.Resolve(entry.Key) is IExpression expr ?
                        $" [{this.Fixup(expr).GetPrettyString(PrettyStringTypes.Readable)}]" :
                        string.Empty)));

        public override string ToString() =>
            "Unifier: " + this.Simple;
        
        [DebuggerStepThrough]
        public static Unifier Create(ITypeCalculator typeCalculator) =>
            new Unifier(typeCalculator);
    }
}
