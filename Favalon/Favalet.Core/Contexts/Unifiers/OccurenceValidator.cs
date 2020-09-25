using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerStepThrough]
    internal sealed class OccurenceValidator
    {
        private readonly Dictionary<IIdentityTerm, IExpression> normalizedUnifications;

        private OccurenceValidator(Dictionary<IIdentityTerm, Unification> unifications) =>
            this.normalizedUnifications = unifications.
                Where(entry => entry.Value.Polarity == UnificationPolarities.Out).
                Select(entry => (id: entry.Key, ex: entry.Value.Expression)).
                Concat(unifications.
                    Where(entry =>
                        (entry.Value.Polarity == UnificationPolarities.In) &&
                        (entry.Value.Expression is IIdentityTerm)).
                    Select(entry => (id: (IIdentityTerm)entry.Value.Expression, ex: (IExpression)entry.Key))).
                ToDictionary(
                    entry => entry.id,
                    entry => entry.ex,
                    IdentityTermComparer.Instance);

        private void Validate(PlaceholderMarker marker, IExpression expression)
        {
            if (expression is IPlaceholderTerm identity)
            {
                this.Validate(marker, identity);
            }
            else if (expression is IFunctionExpression(IExpression parameter, IExpression result))
            {
                this.Validate(marker.Fork(), parameter);
                this.Validate(marker.Fork(), result);
            }
            else if (expression is IBinaryExpression(IExpression left, IExpression right))
            {
                this.Validate(marker.Fork(), left);
                this.Validate(marker.Fork(), right);
            }
        }

        private void Validate(PlaceholderMarker marker, IIdentityTerm identity)
        {
            var targetIdentity = identity;
            while (true)
            {
                if (marker.Mark(targetIdentity))
                {
                    if (this.normalizedUnifications.TryGetValue(targetIdentity, out var resolved))
                    {
                        if (resolved is IPlaceholderTerm placeholder)
                        {
                            targetIdentity = placeholder;
                            continue;
                        }
                        else
                        {
                            Validate(marker, (IIdentityTerm)resolved);
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
        
        public static void Validate(
            IIdentityTerm identity,
            Dictionary<IIdentityTerm, Unification> unifications) =>
            new OccurenceValidator(unifications).
                Validate(PlaceholderMarker.Create(), identity);
    }
}