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

        #region Occurrence
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
        #endregion

        private void InternalUnify(
            IInferContext context,
            IExpression from,
            IExpression to)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));


            var combined = useAndIfNotQualified ?
                (IExpression)AndExpression.Create(from, to) :
                OrExpression.Create(from, to);
            var calculated = context.TypeCalculator.Compute(combined);

            var rewritable = context.MakeRewritable(calculated);
            var inferred = context.Infer(rewritable);

            return inferred;
        }

        public void Unify(
            IInferContext context,
            IExpression from,
            IExpression to)
        {
            // Same as.
            if (context.TypeCalculator.ExactEquals(from, to))
            {
                return;
            }

            switch (from, to)
            {
                // Ignore IIgnoreUnificationTerm unification.
                case (IIgnoreUnificationTerm _, _):
                case (_, IIgnoreUnificationTerm _):
                    return;

                default:
                    // Unify higher order (ignored result.)
                    this.Unify(
                        context, from.HigherOrder, to.HigherOrder);

                    // Unify.
                    this.InternalUnify(
                        context, from, to);
            }
        }

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
