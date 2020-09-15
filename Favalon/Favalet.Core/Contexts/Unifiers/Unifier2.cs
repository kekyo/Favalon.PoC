﻿using Favalet.Expressions;
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

        private void ValidateSubTyping(
            IInferContext context,
            IExpression from,
            IExpression to)
        {
            // from <: to
            var combined = OrExpression.Create(from, to);
            var calculated = context.TypeCalculator.Compute(combined);
            if (!context.TypeCalculator.Equals(calculated, to))
            {
                throw new ArgumentException("");
            }
        }
          
        private void InternalUnifyBothPlaceholders(
            IInferContext context,
            IPlaceholderTerm from,
            IPlaceholderTerm to)
        {
            var ufr = this.unifications.TryGetValue(from, out var uf);
            var utr = this.unifications.TryGetValue(to, out var ut);

            switch (ufr, utr)
            {
                case (true, true):
                    this.Unify(context, from, ut.Expression);
                    if ((uf.Polarity == UnificationPolarities.Covariance) &&
                        (ut.Polarity == UnificationPolarities.Contravariance))
                    {
                        this.unifications[to] = Unification.Create(
                            from, UnificationPolarities.Covariance);
                    }
                    else
                    {
                        throw new ArgumentException("");
                    }
                    break;
                
                case (_, true):
                    this.Unify(context, from, ut.Expression);
                    if (ut.Polarity == UnificationPolarities.Contravariance)
                    {
                        this.unifications[to] = Unification.Create(
                            from, UnificationPolarities.Covariance);
                    }
                    else
                    {
                        throw new ArgumentException("");
                    }
                    break;

                case (true, _):
                    this.Unify(context, uf.Expression, to);
                    if (uf.Polarity == UnificationPolarities.Covariance)
                    {
                        this.unifications[from] = Unification.Create(
                            to, UnificationPolarities.Contravariance);
                    }
                    else
                    {
                        throw new ArgumentException("");
                    }
                    break;
                
                default:
                    this.unifications[to] = Unification.Create(
                        from, UnificationPolarities.Covariance);
                    break;
            }
            
            if (this.unifications.TryGetValue(to, out var unification))
            {
            }
            else
            {
                this.unifications[to] = Unification.Create(
                    from, UnificationPolarities.Covariance);
            }
        }
      
        private void InternalUnifyPlaceholder(
            IInferContext context,
            IExpression from,
            IPlaceholderTerm to)
        {
            if (this.unifications.TryGetValue(to, out var unification))
            {
                // unification <: from
                //this.ValidateSubTyping(context, from, unification.Expression);
                this.Unify(context, from, unification.Expression);

                if (unification.Polarity == UnificationPolarities.Contravariance)
                {
                    this.unifications[to] = Unification.Create(
                        from, UnificationPolarities.Covariance);
                }
            }
            else
            {
                this.unifications[to] = Unification.Create(
                    from, UnificationPolarities.Covariance);
            }
        }

        private void InternalUnifyPlaceholder(
            IInferContext context,
            IPlaceholderTerm from,
            IExpression to)
        {
            if (this.unifications.TryGetValue(from, out var unification))
            {
                // unification <: from
                //this.ValidateSubTyping(context, unification.Expression, to);
                this.Unify(context, unification.Expression, to);

                if (unification.Polarity == UnificationPolarities.Covariance)
                {
                    this.unifications[from] = Unification.Create(
                        to, UnificationPolarities.Contravariance);
                }
            }
            else
            {
                this.unifications[from] = Unification.Create(
                    to, UnificationPolarities.Contravariance);
            }
        }
        
        private void InternalUnify(
            IInferContext context,
            IExpression from,
            IExpression to)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            switch (from, to)
            {
                // Placeholder unifications
                case (IPlaceholderTerm fp1, IPlaceholderTerm tp1):
                    this.InternalUnifyBothPlaceholders(context, fp1, tp1);
                    break;
                case (_, IPlaceholderTerm tp2):
                    this.InternalUnifyPlaceholder(context, from, tp2);
                    break;
                case (IPlaceholderTerm fp2, _):
                    this.InternalUnifyPlaceholder(context, fp2, to);
                    break;

                // Function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IFunctionExpression(IExpression tp, IExpression tr)):
                    // unify(C +> A)
                    this.Unify(context, tp, fp);
                    // unify(B +> D)
                    this.Unify(context, fr, tr);
                    break;
                
                default:
                    // from <: to
                    this.ValidateSubTyping(context, from, to);
                    break;
            }
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
                    break;

                default:
                    // Unify higher order (ignored result.)
                    this.Unify(context, from.HigherOrder, to.HigherOrder);

                    // Unify.
                    this.InternalUnify(context, from, to);
                    break;
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
