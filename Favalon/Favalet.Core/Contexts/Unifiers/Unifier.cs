using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerDisplay("{Unifications}")]
    internal sealed class Unifier :
        FixupContext,  // Because used by "Simple" property implementation.
        IUnsafePlaceholderResolver
    {
        private readonly Dictionary<IIdentityTerm, Unification> unifications =
            new Dictionary<IIdentityTerm, Unification>(IdentityTermComparer.Instance);
        
        [DebuggerStepThrough]
        private Unifier(ITypeCalculator typeCalculator) :
            base(typeCalculator)
        {
        }

        private void Occur(PlaceholderMarker marker, IExpression expression)
        {
            if (expression is IPlaceholderTerm identity)
            {
                this.Occur(marker, identity);
            }
            else if (expression is IFunctionExpression(IExpression parameter, IExpression result))
            {
                this.Occur(marker.Fork(), parameter);
                this.Occur(marker.Fork(), result);
            }
            else if (expression is IBinaryExpression(IExpression left, IExpression right))
            {
                this.Occur(marker.Fork(), left);
                this.Occur(marker.Fork(), right);
            }
        }

        private void Occur(PlaceholderMarker marker, IIdentityTerm identity)
        {
            var targetIdentity = identity;
            while (true)
            {
                if (marker.Mark(targetIdentity))
                {
                    if (this.unifications.TryGetValue(targetIdentity, out var resolved))
                    {
                        if (resolved.Expression is IPlaceholderTerm placeholder)
                        {
                            targetIdentity = placeholder;
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

        private void Update(
            IIdentityTerm identity, 
            IExpression expression,
            bool @fixed)
        {
            if (this.unifications.TryGetValue(identity, out var lastUnification))
            {
                var unification = Unification.Create(
                    expression,
                    lastUnification.Fixed || @fixed);     // Derived if already fixed.
                
                if (!lastUnification.Equals(unification))
                {
                    this.unifications[identity] = unification;

                    Debug.WriteLine(
                        $"Unifier.Update: {identity.GetPrettyString(PrettyStringTypes.Readable)}: {lastUnification} ==> {unification}");

                    this.Occur(PlaceholderMarker.Create(), identity);
                }
            }
            else
            {
                var unification = Unification.Create(expression, @fixed);
                this.unifications.Add(identity, unification);

                Debug.WriteLine(
                    $"Unifier.Update: {identity.GetPrettyString(PrettyStringTypes.Readable)}: {unification}");

                this.Occur(PlaceholderMarker.Create(), identity);
            }
        }

        private void InternalUnifyIdentity(
            IInferContext context,
            IIdentityTerm from,
            IExpression to,
            Unification examinedFrom,
            ExpressionAttribute attribute)
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
                    this.Update(from, to, attribute.Fixed);
                
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
                    var rattribute = ExpressionAttribute.Create(examinedFrom.Fixed || attribute.Fixed);
                    
                    if (this.InternalUnify(
                        context, examinedFrom.Expression, to, rattribute) is IExpression result2)
                    {
                        if (!examinedFrom.Fixed)
                        {
                            this.Update(from, result2, rattribute.Fixed);
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

        private void InternalUnifyBothIdentities(
            IInferContext context,
            IIdentityTerm from,
            IIdentityTerm to,
            ExpressionAttribute attribute)
        {
            var rf = this.unifications.TryGetValue(from, out var rfrom);
            var rt = this.unifications.TryGetValue(to, out var rto);
            
            switch (rf, rt)
            {
                case (true, true):
                    var rattribute = ExpressionAttribute.Create(rfrom.Fixed || rto.Fixed || attribute.Fixed);     // Derived if already fixed.
                    if (this.InternalUnify(
                        context,
                        rfrom.Expression,
                        rto.Expression,
                        rattribute) is IExpression result0)
                    {
                        this.Update(from, result0, rattribute.Fixed);
                        this.Update(to, result0, rattribute.Fixed);
                    }
                    break;
                
                case (true, false):
                    this.InternalUnifyIdentity(
                        context, from, to, rfrom, attribute);
                    break;
                
                case (false, true):
                    this.InternalUnifyIdentity(
                        context, to, from, rto, attribute.Reverse());    // Reversed order.
                    break;
                
                default:
                    this.Update(from, to, attribute.Fixed);
                    break;
            }
        }

        private void InternalUnifyIdentity(
            IInferContext context,
            IIdentityTerm from,
            IExpression to,
            ExpressionAttribute attribute)
        {
            if (this.unifications.TryGetValue(from, out var rfrom))
            {
                this.InternalUnifyIdentity(
                    context, from, to, rfrom, attribute);
            }
            else
            {
                this.Update(from, to, attribute.Fixed);
            }
        }

        private bool InternalUnifyIdentity<TIdentityTerm>(
            IInferContext context,
            IExpression from,
            IExpression to,
            ExpressionAttribute attribute)
            where TIdentityTerm : class, IIdentityTerm
        {
            // Interpret identities.
            if (from is TIdentityTerm fi)
            {
                if (to is TIdentityTerm ti)
                {
                    // Identities aren't matched.
                    if (!fi.Identity.Equals(ti.Identity))
                    {
                        // Unify both identities.
                        this.InternalUnifyBothIdentities(
                            context, fi, ti, attribute);
                    }
                }
                else
                {
                    // Unify from identity.
                    this.InternalUnifyIdentity(
                        context, fi, to, attribute);
                }
                return true;
            }
            else if (to is TIdentityTerm ti)
            {
                // Unify to identity.
                this.InternalUnifyIdentity(
                    context, ti, from, attribute.Reverse());    // Reversed order.
                return true;
            }

            return false;
        }

        private bool InternalUnifyLogical<TBinaryExpression>(
            IInferContext context,
            IExpression from,
            IExpression to,
            ExpressionAttribute attribute,
            Func<IExpression, IExpression, TBinaryExpression> creator,
            out IExpression? result)
            where TBinaryExpression : class, IBinaryExpression
        {
            if (from is TBinaryExpression(IExpression flo, IExpression fro))
            {
                var left = this.InternalUnify(
                    context, flo, to, attribute);
                var right = this.InternalUnify(
                    context, fro, to, attribute);

                if (left is IExpression || right is IExpression)
                {
                    var combined = creator(
                        left is IExpression ? left : creator(flo, to),
                        right is IExpression ? right : creator(fro, to));
                    var calculated = context.TypeCalculator.Compute(combined);

                    var rewritable = context.MakeRewritable(calculated);
                    var inferred = context.Infer(rewritable);

                    result = inferred;
                }
                else
                {
                    result = default;
                }
                return true;
            }
            else if (to is TBinaryExpression(IExpression tlo, IExpression tro))
            {
                var left = this.InternalUnify(
                    context, from, tlo, attribute);
                var right = this.InternalUnify(
                    context, from, tro, attribute);

                if (left is IExpression || right is IExpression)
                {
                    var combined = creator(
                        left is IExpression ? left : creator(from, tlo),
                        right is IExpression ? right : creator(from, tro));
                    var calculated = context.TypeCalculator.Compute(combined);

                    var rewritable = context.MakeRewritable(calculated);
                    var inferred = context.Infer(rewritable);

                    result = inferred;
                }
                else
                {
                    result = default;
                }
                return true;
            }

            result = default;
            return false;
        }

        private bool InternalUnifyFunction(
            IInferContext context,
            IExpression from,
            IExpression to,
            ExpressionAttribute attribute,
            out IExpression? result)
        {
            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                // Unify FunctionExpression.
                var parameter = this.InternalUnify(
                    context, fp, tp, attribute);
                var result_ = this.InternalUnify(
                    context, fr, tr, attribute);

                if (parameter is IExpression || result_ is IExpression)
                {
                    result = FunctionExpression.Create(
                        parameter is IExpression ? parameter : fp,
                        result_ is IExpression ? result_ : fr);
                }
                else
                {
                    result = default;
                }
                return true;
            }

            result = default;
            return false;
        }

        private IExpression? InternalUnifyCore(
            IInferContext context,
            IExpression from,
            IExpression to,
            ExpressionAttribute attribute)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            // Interpret placeholders.
            if (this.InternalUnifyIdentity<IPlaceholderTerm>(
                context, from, to, attribute))
            {
                // Done.
                return null;
            }

            // Interpret identities.
            if (this.InternalUnifyIdentity<IIdentityTerm>(
                context, from, to, attribute))
            {
                // Done.
                return null;
            }

            // Or expression.
            if (this.InternalUnifyLogical<IOrExpression>(
                context, from, to, attribute, OrExpression.Create, out var result0))
            {
                // Done and got replacement.
                return result0;
            }
            // And expression.
            else if (this.InternalUnifyLogical<IAndExpression>(
                context, from, to, attribute, AndExpression.Create, out var result1))
            {
                // Done and got replacement.
                return result1;
            }

            if (this.InternalUnifyFunction(
                context, from, to, attribute, out var result2))
            {
                // Done and got replacement.
                return result2;
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
            ExpressionAttribute attribute)
        {
            // Same as.
            if (context.TypeCalculator.ExactEquals(from, to))
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
                context, from, to, ExpressionAttribute.Create(@fixed));

        public override IExpression? Resolve(IIdentityTerm identity)
        {
#if DEBUG
            this.Occur(PlaceholderMarker.Create(), identity);
#endif
            return this.unifications.TryGetValue(identity, out var resolved) ?
                resolved.Expression :
                null;
        }

        public string Unifications
        {
            [DebuggerStepThrough]
            get => StringUtilities.Join(
                Environment.NewLine,
                this.unifications.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    Select(entry => string.Format(
                        "{0} ==> {1}{2}",
                        entry.Key.Symbol,
                        entry.Value.ToString(PrettyStringTypes.Minimum),
                        this.Resolve(entry.Key) is IExpression expr
                            ? $" [{this.Fixup(expr).GetPrettyString(PrettyStringTypes.Readable)}]"
                            : string.Empty)));
        }

        public override string ToString() =>
            "Unifier: " + this.Unifications;
        
        [DebuggerStepThrough]
        public static Unifier Create(ITypeCalculator typeCalculator) =>
            new Unifier(typeCalculator);
    }
}
