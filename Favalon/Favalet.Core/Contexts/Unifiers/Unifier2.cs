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

        private void UpdateUnification(
            IInferContext context,
            IPlaceholderTerm placeholder,
            IExpression expression,
            UnificationPolarities polarity)
        {
            var updated = Unification.Create(expression, polarity);
            
#if DEBUG
            if (this.unifications.TryGetValue(placeholder, out var last))
            {
                if (context.TypeCalculator.Equals(
                    last.Expression, updated.Expression) &&
                    (last.Polarity == updated.Polarity))
                {
                }
                else
                {
                    Debug.WriteLine(
                        $"UpdateUnification: {placeholder} ==> [{last} --> {updated}]");
                }
            }
            else
            {
                Debug.WriteLine(
                    $"UpdateUnification: {placeholder} ==> {updated}");
            }
#endif
            this.unifications[placeholder] = updated;
            
            this.Occur(PlaceholderMarker.Create(), placeholder);
        }

        private void InternalUnifyBothPlaceholders(
            IInferContext context,
            IPlaceholderTerm from,
            IPlaceholderTerm to)
        {
            var fr = this.unifications.TryGetValue(from, out var flast);
            var tr = this.unifications.TryGetValue(to, out var tlast);

            switch (fr, tr)
            {
                case (true, true):
                    switch (flast.Polarity, tlast.Polarity)
                    {
                        case (UnificationPolarities.Out, UnificationPolarities.In):
                            this.UpdateUnification(context, to, from, UnificationPolarities.Out);
                            this.Unify(context, from, tlast.Expression);
                            break;

                        case (UnificationPolarities.In, UnificationPolarities.Out):
                            this.UpdateUnification(context, from, to, UnificationPolarities.In);
                            this.Unify(context, flast.Expression, to);
                            break;

                        case (UnificationPolarities.Out, UnificationPolarities.Out):
                            this.Unify(context, from, tlast.Expression);
                            break;
                        
                        case (UnificationPolarities.In, UnificationPolarities.In):
                            this.Unify(context, flast.Expression, to);
                            break;
                    }
                    break;
                
                case (_, true):
                    switch (tlast.Polarity)
                    {
                        case UnificationPolarities.In:
                            this.UpdateUnification(context, to, from, UnificationPolarities.Out);
                            this.Unify(context, from, tlast.Expression);
                            break;
                        
                        case UnificationPolarities.Out:
                            this.Unify(context, from, tlast.Expression);
                            break;
                    }
                    break;

                case (true, _):
                    switch (flast.Polarity)
                    {
                        case UnificationPolarities.Out:
                            this.UpdateUnification(context, from, to, UnificationPolarities.In);
                            this.Unify(context, flast.Expression, to);
                            break;
                        
                        case UnificationPolarities.In:
                            this.Unify(context, flast.Expression, to);
                            break;
                    }
                    break;
                
                default:
                    this.UpdateUnification(context, to, from, UnificationPolarities.Out);
                    break;
            }
        }
      
        private void InternalUnifyPlaceholderForward(
            IInferContext context,
            IExpression from,
            IPlaceholderTerm placeholder)
        {
            if (this.unifications.TryGetValue(placeholder, out var last))
            {
                switch (last.Polarity)
                {
                    case UnificationPolarities.In:
                        this.UpdateUnification(context, placeholder, from, UnificationPolarities.Out);
                        this.Unify(context, last.Expression, from);
                        break;
                    
                    case UnificationPolarities.Out:
                        this.Unify(context, from, last.Expression);
                        break;
                }
            }
            else
            {
                this.UpdateUnification(context, placeholder, from, UnificationPolarities.Out);
            }
        }

        private void InternalUnifyPlaceholderBackward(
            IInferContext context,
            IPlaceholderTerm placeholder,
            IExpression to)
        {
            if (this.unifications.TryGetValue(placeholder, out var last))
            {
                switch (last.Polarity)
                {
                    case UnificationPolarities.Out:
                        this.UpdateUnification(context, placeholder, to, UnificationPolarities.In);
                        this.Unify(context, last.Expression, placeholder);
                        break;
                    
                    case UnificationPolarities.In:
                        this.Unify(context, last.Expression, to);
                        break;
                }
            }
            else
            {
                this.UpdateUnification(context, placeholder, to, UnificationPolarities.In);
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
                // Placeholder unification.
                case (IPlaceholderTerm fp1, IPlaceholderTerm tp1):
                    this.InternalUnifyBothPlaceholders(context, fp1, tp1);
                    break;
                case (_, IPlaceholderTerm tp2):
                    this.InternalUnifyPlaceholderForward(context, from, tp2);
                    break;
                case (IPlaceholderTerm fp2, _):
                    this.InternalUnifyPlaceholderBackward(context, fp2, to);
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
                    // Validate polarity.
                    // from <: to
                    var f = context.TypeCalculator.Compute(OrExpression.Create(from, to));
                    if (!context.TypeCalculator.Equals(f, to))
                    {
                        throw new ArgumentException("");
                    }
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
                    // Unify higher order.
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
