using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal enum LeaveScopeResults
    {
        None,
        Implicitly,
        Explicitly
    }

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
                        // (Dodge SavedTerm)
                        if (context.CurrentTerm is ApplyTerm(Term left, Term right))
                        {
                            // Swap (unapply) and begin new implicitly scope
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
                        // Leave one implicit scope 
                        // And disable deconstruct last ApplyTerm if didn't scope out
                        LeaveOneImplicitScope(context);
                    }
                }

                // Update precedence
                context.CurrentPrecedence = terms[0].Precedence;

                if (terms[0].Infix)
                {
                    // "abc def +" ==> "abc + def"
                    // (Dodge SavedTerm)
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

        public static LeaveScopeResults LeaveOneImplicitScope(ParseRunnerContext context)
        {
            if (context.Scopes.Count >= 1)
            {
                // Get last parenthesis scope:
                var parenthesisScope = context.Scopes.Pop();

                // Did this scope have parenthesis?
                if (parenthesisScope.ParenthesisPair is ParenthesisPair scopeParenthesisPair)
                {
                    throw new InvalidOperationException(
                        $"Unmatched parenthesis: Opened={scopeParenthesisPair.Open}");
                }
                // Implicit (RTL) scope:
                else
                {
                    // Combine it implicitly.
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);

                    // Reset precedence, because finished a scope.
                    context.CurrentPrecedence = null;

                    // Leave Implicit scope.
                    return LeaveScopeResults.Implicitly;
                }
            }
            else
            {
                // Make term hiding:
                // because invalid deconstruction ApplyTerm for next token iteration.
                if (context.CurrentTerm is ApplyTerm t)
                {
                    context.CurrentTerm = new HideTerm(t);
                }

                // Matching scope didn't find
                return LeaveScopeResults.None;
            }
        }

        public static LeaveScopeResults LeaveOneScope(
            ParseRunnerContext context, ParenthesisPair parenthesisPair)
        {
            if (context.Scopes.Count >= 1)
            {
                // Get last parenthesis scope:
                var parenthesisScope = context.Scopes.Pop();
                if (parenthesisScope.ParenthesisPair is ParenthesisPair scopeParenthesisPair)
                {
                    // Is parenthesis not matching
                    if (scopeParenthesisPair.Close != parenthesisPair.Close)
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesisPair.Close}, Opened={scopeParenthesisPair.Open}");
                    }

                    // Parenthesis scope:
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);

                    // Reset precedence, because finished a scope.
                    context.CurrentPrecedence = null;

                    // Matched scope
                    return LeaveScopeResults.Explicitly;
                }
                // Implicit (RTL) scope:
                else
                {
                    // Combine it implicitly.
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);

                    // Reset precedence, because finished a scope.
                    context.CurrentPrecedence = null;

                    // Leave Implicit scope.
                    return LeaveScopeResults.Implicitly;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unmatched parenthesis: {parenthesisPair.Close}");
            }
        }

        public static Term FinalizeHideTerm(Term term)
        {
            switch (term)
            {
                // Unveil HideTerm recursivity
                case HideTerm(Term hideTerm):
                    return FinalizeHideTerm(hideTerm);

                case ApplyTerm(Term function, Term argument):
                    var f = FinalizeHideTerm(function);
                    var a = FinalizeHideTerm(argument);
                    if (!object.ReferenceEquals(f, function) ||
                        !object.ReferenceEquals(a, argument))
                    {
                        return new ApplyTerm(f, a);
                    }
                    else
                    {
                        return term;
                    }

                default:
                    return term;
            }
        }
    }
}
