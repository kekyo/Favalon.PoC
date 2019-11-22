using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal static class ParserUtilities
    {
#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Term CombineTerms(Term? left, Term? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return new ApplyTerm(left, right);
                }
                else
                {
                    return left;
                }
            }
            else
            {
                return right!;
            }
        }

        public static Term CombineTerms(params Term?[] terms) =>
            terms.Aggregate(CombineTerms)!;

        public static ConstantTerm GetNumericConstant(string value, NumericalSignes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);

        public static ParseRunnerResult RunIdentity(ParseRunnerContext context, IdentityToken identity)
        {
            if (context.Context.LookupBoundTerms(identity.Identity) is BoundTermInformation[] terms)
            {
                // Not first time
                if (context.CurrentPrecedence is BoundTermPrecedences precedence)
                {
                    // Greater than current precedence
                    if (terms[0].Precedence > precedence)
                    {
                        // Update precedence
                        context.CurrentPrecedence = terms[0].Precedence;

                        if (context.CurrentTerm is ApplyTerm(Term left, Term right))
                        {
                            // Unapply and begin new implicitly scope
                            // "+ abc def   *" ==> "+ abc (def   *"
                            //  left  right id      left  (right id
                            context.CurrentTerm = left;
                            context.PushScope();

                            // Set first term from right
                            context.CurrentTerm = right;
                        }
                        else
                        {
                            // Begin new implicitly scope
                            // "+ *" ==> "+ (*"
                            context.PushScope();
                        }
                    }
                    // Lesser than current precedence
                    else if (terms[0].Precedence < precedence)
                    {
                        // Update precedence
                        context.CurrentPrecedence = terms[0].Precedence;

                        // Leave one implicitly scope
                        LeaveScope(context, null);
                    }
                }
                // First time
                else
                {
                    // Update precedence
                    context.CurrentPrecedence = terms[0].Precedence;
                }

                if (terms[0].Infix)
                {
                    // "abc def +" ==> "abc + def"
                    if (context.CurrentTerm is ApplyTerm(Term left, Term right))
                    {
                        context.CurrentTerm = CombineTerms(
                            left,
                            new IdentityTerm(identity.Identity),
                            right);
                    }
                    // "abc +" ==> "+ abc"
                    else
                    {
                        context.CurrentTerm = CombineTerms(
                            new IdentityTerm(identity.Identity),
                            context.CurrentTerm);
                    }
                }
                else
                {
                    // Will not swap
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(identity.Identity));
                }

                // Update precedence
                context.CurrentPrecedence = terms[0].Precedence;

                // Pre marking RTL
                context.WillApplyRightToLeft = terms[0].RightToLeft;
            }
            // "abc def"
            else
            {
                context.CurrentTerm = CombineTerms(
                    context.CurrentTerm,
                    new IdentityTerm(identity.Identity));
            }
            return ParseRunnerResult.Empty(ApplyingRunner.Instance);
        }

        public static bool LeaveScope(
            ParseRunnerContext context, ParenthesisPair? parenthesisPair)
        {
            if (context.Scopes.Count >= 1)
            {
                // Get last parenthesis scope:
                var parenthesisScope = context.Scopes.Pop();
                if (parenthesisScope.ParenthesisPair is ParenthesisPair scopeParenthesisPair)
                {
                    // Is parenthesis not matching
                    if (!parenthesisPair.HasValue ||
                        scopeParenthesisPair.Close != parenthesisPair.Value.Close)
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesisPair}");
                    }

                    // Combine it
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);
                    return true;
                }
                // RTL scope:
                else
                {
                    // Combine it implicitly.
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);
                }
            }

            // Matching scope didn't find
            return false;
        }

        public static bool LeaveScopes(
            ParseRunnerContext context, ParenthesisPair? parenthesisPair)
        {
            while (context.Scopes.Count >= 1)
            {
                if (LeaveScope(context, parenthesisPair))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
