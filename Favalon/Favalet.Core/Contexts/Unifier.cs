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
        FixupContext  // Because used by "Simple" property implementation.
    {
        private readonly Dictionary<int, IExpression> unifications =
            new Dictionary<int, IExpression>();

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
                        if (resolved is IPlaceholderTerm placeholder)
                        {
                            targetIndex = placeholder.Index;
                            continue;
                        }
                        else
                        {
                            this.Occur(marker, resolved);
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

        private void Update(int index, IExpression expression)
        {
#if DEBUG
            if (this.unifications.TryGetValue(index, out var origin))
            {
                if (!origin.Equals(expression))
                {
                    Debug.WriteLine(
                        $"Unifier.Update: '{index}: {origin.GetPrettyString(PrettyStringTypes.Readable)} ==> {expression.GetPrettyString(PrettyStringTypes.Readable)}");
                }
            }
#endif
            this.unifications[index] = expression;
            
            this.Occur(PlaceholderMarker.Create(), index);
        }

        private IExpression? InternalUnifyBothPlaceholders(
            IInferContext context,
            IPlaceholderTerm from,
            IPlaceholderTerm to,
            UnifyConstraints fromConstraint,
            UnifyConstraints toConstraint)
        {
            this.unifications.TryGetValue(from.Index, out var rfrom);
            this.unifications.TryGetValue(to.Index, out var rto);
            
            switch (rfrom, rto)
            {
                case (IExpression _, IExpression _):
                    if (this.InternalUnify(
                        context,
                        rfrom,
                        rto,
                        fromConstraint,
                        toConstraint) is IExpression result0)
                    {
                        this.Update(from.Index, result0);
                        this.Update(to.Index, result0);
                    }
                    return null;
                
                case (IExpression _, null):
                    if (fromConstraint == UnifyConstraints.Fixed)
                    {
                        this.Update(from.Index, to);
                        if (this.InternalUnify(
                            context,
                            rfrom,
                            to,
                            UnifyConstraints.Free,
                            /* derived from */ fromConstraint) is IExpression result)
                        {
                            var combined = OrExpression.Create(to, result);
                            var calculated = context.TypeCalculator.Compute(combined);

                            if (!calculated.Equals(result))
                            {
                                throw new InvalidOperationException(
                                    $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
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
                            toConstraint) is IExpression result)
                        {
                            this.Update(from.Index, result);
                        }
                    }
                    return null;
                
                case (null, IExpression _):
                    if (toConstraint == UnifyConstraints.Fixed)
                    {
                        this.Update(to.Index, from);
                        if (this.InternalUnify(
                            context,
                            from,
                            rto,
                            /* derived to */ toConstraint,
                            UnifyConstraints.Free) is IExpression result)
                        {
                            var combined = OrExpression.Create(from, result);
                            var calculated = context.TypeCalculator.Compute(combined);

                            if (!calculated.Equals(result))
                            {
                                throw new InvalidOperationException(
                                    $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
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
                            toConstraint) is IExpression result)
                        {
                            this.Update(to.Index, result);
                        }
                    }
                    return null;
                
                default:
                    this.Update(from.Index, to);
                    return null;
            }
        }

        private IExpression? InternalUnifyPlaceholder(
            IInferContext context,
            IPlaceholderTerm from,
            IExpression to,
            UnifyConstraints fromConstraint,
            UnifyConstraints toConstraint)
        {
            if (this.unifications.TryGetValue(from.Index, out var target))
            {
                if (fromConstraint == UnifyConstraints.Fixed)
                {
                    this.Update(from.Index, to);
                    if (this.InternalUnify(
                        context,
                        to, 
                        target,
                        /* derived from */ fromConstraint,
                        UnifyConstraints.Free) is IExpression result)
                    {
                        var combined = OrExpression.Create(to, result);
                        var calculated = context.TypeCalculator.Compute(combined);

                        if (!calculated.Equals(result))
                        {
                            throw new InvalidOperationException(
                                $"Cannot unify: {from.GetPrettyString(PrettyStringTypes.Readable)} ==> {to.GetPrettyString(PrettyStringTypes.Readable)}");
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
                        toConstraint) is IExpression result)
                    {
                        this.Update(from.Index, result);
                    }
                }
            }
            else
            {
                this.Update(from.Index, to);
            }

            return null;
        }

        private IExpression? InternalUnifyCore(
            IInferContext context,
            IExpression from,
            IExpression to,
            UnifyConstraints fromConstraint,
            UnifyConstraints toConstraint)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            // Interpret placeholders.
            if (from is IPlaceholderTerm(int fromIndex) fph)
            {
                if (to is IPlaceholderTerm(int toIndex) tph)
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
                            fph,
                            tph,
                            fromConstraint,
                            toConstraint);
                    }
                }
                else
                {
                    // Unify from placeholder.
                    return this.InternalUnifyPlaceholder(
                        context,
                        fph, 
                        to,
                        fromConstraint,
                        toConstraint);
                }
            }
            else if (to is IPlaceholderTerm tph)
            {
                // Unify to placeholder.
                return this.InternalUnifyPlaceholder(
                    context,
                    tph,             // Reversed order.
                    from,
                    toConstraint,    // Reversed order.
                    fromConstraint);
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var parameter = this.InternalUnify(
                    context,
                    fp,
                    tp,
                    fromConstraint,  // TODO: covariance?
                    toConstraint);
                var result = this.InternalUnify(
                    context,
                    fr,
                    tr,
                    fromConstraint,  // TODO: covariance?
                    toConstraint);

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

            var combined = OrExpression.Create(from, to);
            var calculated = context.TypeCalculator.Compute(combined);

            var rewritable = context.MakeRewritable(calculated);
            var inferred = context.Infer(rewritable);

            return inferred;
        }

        private IExpression? InternalUnify(
            IInferContext context,
            IExpression from,
            IExpression to,
            UnifyConstraints fromConstraint,
            UnifyConstraints toConstraint)
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
                    // Higher order unification.
                    this.InternalUnify(
                        context,
                        from.HigherOrder,
                        to.HigherOrder,
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
                from,
                to,
                fixedAssignes ? UnifyConstraints.Fixed : UnifyConstraints.Free,
                fixedAssignes ? UnifyConstraints.Fixed : UnifyConstraints.Free);

        public override IExpression? Resolve(int index)
        {
#if DEBUG
            this.Occur(PlaceholderMarker.Create(), index);
#endif
            return this.unifications.TryGetValue(index, out var resolved) ? resolved : null;
        }

        public string Xml =>
            new XElement(
                "Unifier",
                this.unifications.OrderBy(entry => entry.Key).Select(entry => new XElement("Unification",
                    new XAttribute("symbol", entry.Key),
                    entry.Value.GetXml())).Memoize()).ToString();
        
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
