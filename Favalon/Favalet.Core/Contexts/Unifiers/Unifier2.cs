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
                        $"UpdateUnification: {placeholder} {updated} [{last}]");
                }
            }
            else
            {
                Debug.WriteLine(
                    $"UpdateUnification: {placeholder} {updated}");
            }
#endif
            this.unifications[placeholder] = updated;
            
            OccurenceValidator.Validate(placeholder, this.unifications);
        }

        private void InternalUnifyBothPlaceholders(
            IInferContext context,
            IPlaceholderTerm from,
            IPlaceholderTerm to)
        {
            var fr = this.unifications.TryGetValue(from, out var flast);
            var tr = this.unifications.TryGetValue(to, out var tlast);

            // Detect already saved unification pair.
            if (fr &&
                (flast.Polarity == UnificationPolarities.In) &&
                context.TypeCalculator.Equals(flast.Expression, to))
            {
                return;
            }
            if (tr &&
                (tlast.Polarity == UnificationPolarities.Out) &&
                context.TypeCalculator.Equals(from, tlast.Expression))
            {
                return;
            }
               
            switch (fr, tr)
            {
                // Make recursive unification when both expressions are placeholder,
                // uses only forward direction strategy.
                case (true, true):
                    this.UpdateUnification(context, to, from, UnificationPolarities.Out);
                    switch (tlast.Polarity)
                    {
                        case UnificationPolarities.In:
                            this.Unify(context, to, tlast.Expression);
                            break;

                        case UnificationPolarities.Out:
                            this.Unify(context, tlast.Expression, to);
                            break;
                    }
                    break;
                
                case (_, true):
                    // Make backward direction when detect non-saved placeholder.
                    this.UpdateUnification(context, from, to, UnificationPolarities.In);
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
            if (this.unifications.TryGetValue(placeholder, out var tlast))
            {
                // Uses only forward direction strategy.
                this.UpdateUnification(context, placeholder, from, UnificationPolarities.Out);
                switch (tlast.Polarity)
                {
                    case UnificationPolarities.In:
                        this.Unify(context, tlast.Expression, placeholder);
                        break;
                    
                    case UnificationPolarities.Out:
                        this.Unify(context, placeholder, tlast.Expression);
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
            if (this.unifications.TryGetValue(placeholder, out var flast))
            {
                // Uses only backward direction strategy.
                this.UpdateUnification(context, placeholder, to, UnificationPolarities.In);
                switch (flast.Polarity)
                {
                    case UnificationPolarities.In:
                        this.Unify(context, placeholder, flast.Expression);
                        break;
                    
                    case UnificationPolarities.Out:
                        this.Unify(context, flast.Expression, placeholder);
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
            OccurenceValidator.Validate(identity, this.unifications);
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
                        "{0} {1}{2}",
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
