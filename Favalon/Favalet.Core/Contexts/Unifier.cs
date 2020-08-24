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
    internal enum Constraints
    {
        Free,
        Fixed
    }

    [DebuggerStepThrough]
    internal readonly struct ConstrainedExpression
    {
        public readonly IExpression Expression;
        public readonly Constraints Constraint;
        
        private ConstrainedExpression(IExpression expression, Constraints constraint)
        {
            this.Expression = expression;
            this.Constraint = constraint;
        }

        public string GetPrettyString(PrettyStringTypes type) =>
            $"{this.Expression.GetPrettyString(type)},{this.Constraint}";
        public override string ToString() =>
            this.GetPrettyString(PrettyStringTypes.Readable);

        public static ConstrainedExpression Create(IExpression expression) =>
            new ConstrainedExpression(expression, Constraints.Free);
        public static ConstrainedExpression Create(IExpression expression, Constraints constraint) =>
            new ConstrainedExpression(expression, constraint);
    }
    
    [DebuggerDisplay("{Simple}")]
    internal sealed class Unifier :
        FixupContext  // Because used by "Simple" property implementation.
    {
        private readonly Dictionary<int, ConstrainedExpression> unifications =
            new Dictionary<int, ConstrainedExpression>();

        [DebuggerStepThrough]
        private Unifier(ILogicalCalculator typeCalculator) :
            base(typeCalculator)
        {
        }

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

        private void Update(int index, ConstrainedExpression expression)
        {
#if DEBUG
            if (this.unifications.TryGetValue(index, out var origin))
            {
                if (!origin.Equals(expression))
                {
                    Debug.WriteLine(
                        $"Unifier.Update: '{index}: {origin} ==> {expression}");
                }
            }
#endif
            this.unifications[index] = expression;
            
            this.Occur(PlaceholderMarker.Create(), index);
        }

        private ConstrainedExpression? InternalUnifyBothPlaceholders(
            IInferContext context,
            ConstrainedExpression from,
            ConstrainedExpression to,
            Constraints fromConstraint,
            Constraints toConstraint)
        {
            Debug.Assert(from.Expression is IPlaceholderTerm);
            Debug.Assert(to.Expression is IPlaceholderTerm);
            
            var f = this.unifications.TryGetValue(
                ((IPlaceholderTerm)from.Expression).Index,
                out var rfrom);
            var t = this.unifications.TryGetValue(
                ((IPlaceholderTerm)to.Expression).Index,
                out var rto);
            
            switch (f, t)
            {
                case (true, true):
                    if (this.InternalUnify(
                        context,
                        rfrom,
                        rto,
                        fromConstraint,
                        toConstraint) is ConstrainedExpression result0)
                    {
                        this.Update(((IPlaceholderTerm)from.Expression).Index, result0);
                        this.Update(((IPlaceholderTerm)to.Expression).Index, result0);
                    }
                    return null;
                
                case (true, false):
                    if (fromConstraint == Constraints.Fixed)
                    {
                        this.Update(((IPlaceholderTerm)from.Expression).Index, to);
                        if (this.InternalUnify(
                            context,
                            rfrom,
                            to,
                            Constraints.Free,
                            /* derived from */ fromConstraint) is ConstrainedExpression result)
                        {
                            var combined = OrExpression.Create(to.Expression, result.Expression);
                            var calculated = context.TypeCalculator.Compute(combined);

                            if (!calculated.Equals(result))
                            {
                                throw new InvalidOperationException(
                                    $"Cannot unify: {from} ==> {to}");
                            }
                        }
                    }
                    else
                    {
                        if (this.InternalUnify(
                            context,
                            rfrom,
                            to,
                            fromConstraint,
                            toConstraint) is ConstrainedExpression result)
                        {
                            this.Update(((IPlaceholderTerm)from.Expression).Index, result);
                        }
                    }
                    return null;
                
                case (false, true):
                    if (toConstraint == Constraints.Fixed)
                    {
                        this.Update(((IPlaceholderTerm)to.Expression).Index, from);
                        if (this.InternalUnify(
                            context,
                            from,
                            rto,
                            /* derived to */ toConstraint,
                            Constraints.Free) is ConstrainedExpression result)
                        {
                            var combined = OrExpression.Create(from.Expression, result.Expression);
                            var calculated = context.TypeCalculator.Compute(combined);

                            if (!calculated.Equals(result))
                            {
                                throw new InvalidOperationException(
                                    $"Cannot unify: {from} ==> {to}");
                            }
                        }
                    }
                    else
                    {
                        if (this.InternalUnify(
                            context,
                            from,
                            rto,
                            fromConstraint,
                            toConstraint) is ConstrainedExpression result)
                        {
                            this.Update(((IPlaceholderTerm)to.Expression).Index, result);
                        }
                    }
                    return null;
                
                default:
                    this.Update(((IPlaceholderTerm)from.Expression).Index, to);
                    return null;
            }
        }

        private ConstrainedExpression? InternalUnifyPlaceholder(
            IInferContext context,
            ConstrainedExpression from,
            ConstrainedExpression to,
            Constraints fromConstraint,
            Constraints toConstraint)
        {
            Debug.Assert(from.Expression is IPlaceholderTerm);

            if (this.unifications.TryGetValue(((IPlaceholderTerm)from.Expression).Index, out var target))
            {
                if (fromConstraint == Constraints.Fixed)
                {
                    this.Update(((IPlaceholderTerm)from.Expression).Index, to);
                    if (this.InternalUnify(
                        context,
                        to, 
                        target,
                        /* derived from */ fromConstraint,
                        Constraints.Free) is ConstrainedExpression result)
                    {
                        var combined = OrExpression.Create(to.Expression, result.Expression);
                        var calculated = context.TypeCalculator.Compute(combined);

                        if (!calculated.Equals(result))
                        {
                            throw new InvalidOperationException(
                                $"Cannot unify: {from} ==> {to}");
                        }
                    }
                }
                else
                {
                    if (this.InternalUnify(
                        context,
                        to,
                        target,
                        fromConstraint,
                        toConstraint) is ConstrainedExpression result)
                    {
                        this.Update(((IPlaceholderTerm)from.Expression).Index, result);
                    }
                }
            }
            else
            {
                this.Update(((IPlaceholderTerm)from.Expression).Index, to);
            }

            return null;
        }

        private ConstrainedExpression? InternalUnifyCore(
            IInferContext context,
            ConstrainedExpression from,
            ConstrainedExpression to,
            Constraints fromConstraint,
            Constraints toConstraint)
        {
            Debug.Assert(!(from.Expression is IIgnoreUnificationTerm));
            Debug.Assert(!(to.Expression is IIgnoreUnificationTerm));

            // Interpret placeholders.
            if (from.Expression is IPlaceholderTerm(int fromIndex))
            {
                if (to.Expression is IPlaceholderTerm(int toIndex))
                {
                    // [1]
                    if (fromIndex == toIndex)
                    {
                        // Ignore equal placeholders.
                        return null;
                    }
                    else
                    {
                        // Unify both placeholders.
                        return this.InternalUnifyBothPlaceholders(
                            context,
                            from,
                            to,
                            fromConstraint,
                            toConstraint);
                    }
                }
                else
                {
                    // Unify from placeholder.
                    return this.InternalUnifyPlaceholder(
                        context,
                        from, 
                        to,
                        fromConstraint,
                        toConstraint);
                }
            }
            else if (to.Expression is IPlaceholderTerm)
            {
                // Unify to placeholder.
                return this.InternalUnifyPlaceholder(
                    context,
                    to,             // Reversed order.
                    from,
                    toConstraint,    // Reversed order.
                    fromConstraint);
            }

            if (from.Expression is IFunctionExpression(IExpression fp, IExpression fr) &&
                to.Expression is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var parameter = this.InternalUnify(
                    context,
                    ConstrainedExpression.Create(fp),  // TODO: covariance?
                    ConstrainedExpression.Create(tp),
                    fromConstraint,
                    toConstraint);
                var result = this.InternalUnify(
                    context,
                    ConstrainedExpression.Create(fr),  // TODO: covariance?
                    ConstrainedExpression.Create(tr),
                    fromConstraint,
                    toConstraint);

                if (parameter?.Expression is IExpression || result?.Expression is IExpression)
                {
                    return ConstrainedExpression.Create(
                        FunctionExpression.Create(
                            parameter?.Expression ?? fp,
                            result?.Expression ?? fr));
                }
                else
                {
                    return null;
                }
            }

            var combined = OrExpression.Create(from.Expression, to.Expression);
            var calculated = context.TypeCalculator.Compute(combined);

            var rewritable = context.MakeRewritable(calculated);
            var inferred = context.Infer(rewritable);

            return ConstrainedExpression.Create(inferred);
        }

        private ConstrainedExpression? InternalUnify(
            IInferContext context,
            ConstrainedExpression from,
            ConstrainedExpression to,
            Constraints fromConstraint,
            Constraints toConstraint)
        {
            // Same as.
            if (object.ReferenceEquals(from.Expression, to.Expression))
            {
                return null;
            }

            switch (from.Expression, to.Expression)
            {
                // Ignore IIgnoreUnificationTerm unification.
                case (IIgnoreUnificationTerm _, _):
                case (_, IIgnoreUnificationTerm _):
                    return null;

                default:
                    // Higher order unification.
                    this.InternalUnify(
                        context,
                        ConstrainedExpression.Create(from.Expression.HigherOrder),
                        ConstrainedExpression.Create(to.Expression.HigherOrder),
                        fromConstraint,
                        toConstraint);

                    // Unification.
                    return this.InternalUnifyCore(
                        context,
                        from,
                        to,
                        fromConstraint,
                        toConstraint);
            }
        }

        [DebuggerStepThrough]
        public void Unify(
            IInferContext context,
            IExpression from,
            IExpression to,
            bool fixedAssignes) =>
            this.InternalUnify(
                context,
                ConstrainedExpression.Create(from),
                ConstrainedExpression.Create(to),
                fixedAssignes ? Constraints.Fixed : Constraints.Free,
                fixedAssignes ? Constraints.Fixed : Constraints.Free);

        public override IExpression? Resolve(int index)
        {
#if DEBUG
            this.Occur(PlaceholderMarker.Create(), index);
#endif
            return this.unifications.TryGetValue(index, out var resolved) ? resolved.Expression : null;
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
                    "{0} --> {1}{2}",
                    entry.Key,
                    entry.Value.GetPrettyString(PrettyStringTypes.ReadableWithoutHigherOrder),
                    this.Resolve(entry.Key) is IExpression expr ?
                        $" [{this.Fixup(expr).GetPrettyString(PrettyStringTypes.Readable)}]" :
                        string.Empty)));

        public override string ToString() =>
            "Unifier: " + this.Simple;
        
        [DebuggerStepThrough]
        public static Unifier Create(ILogicalCalculator typeCalculator) =>
            new Unifier(typeCalculator);
    }
}
