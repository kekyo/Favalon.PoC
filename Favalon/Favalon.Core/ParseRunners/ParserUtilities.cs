﻿using Favalon.Terms;
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

        public static Term? HideTerm(Term? term) =>
            term is ApplyTerm apply ? new HidedApplyTerm(apply) : term;

        public static ConstantTerm GetNumericConstant(string value, NumericalSignes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);

        public static ParseRunnerResult RunIdentity(ParseRunnerContext runnerContext, IdentityToken identity)
        {
            if (runnerContext.CurrentContext.LookupBoundTerms(identity.Identity) is BoundTermInformation[] terms)
            {
                // Not first time
                if (runnerContext.CurrentPrecedence is BoundTermPrecedences precedence)
                {
                    // Greater than current precedence
                    if (terms[0].Precedence > precedence)
                    {
                        // (Dodge SavedTerm)
                        if (runnerContext.CurrentTerm is ApplyTerm(Term left, Term right))
                        {
                            // Swap (unapply) and begin new implicitly scope
                            // "+ abc def   *" ==> "+ abc (def   *"
                            //  left  right id      left  (right id
                            runnerContext.CurrentTerm = left;
                            runnerContext.PushScope();

                            // Set first term from right
                            runnerContext.CurrentTerm = right;
                        }
                        else
                        {
                            // Begin new implicitly scope
                            // "+ *" ==> "+ (*"
                            runnerContext.PushScope();
                        }
                    }
                    // Lesser than current precedence
                    else if (terms[0].Precedence < precedence)
                    {
                        // Leave one implicit scope 
                        // And disable deconstruct last ApplyTerm if didn't scope out
                        LeaveOneImplicitScope(runnerContext);
                    }
                }

                // Update precedence
                runnerContext.CurrentPrecedence = terms[0].Precedence;

                if (terms[0].Notation == BoundTermNotations.Infix)
                {
                    // "abc def +" ==> "abc + def"
                    // (Dodge SavedTerm)
                    if (runnerContext.CurrentTerm is ApplyTerm(Term left, Term right))
                    {
                        runnerContext.CurrentTerm = CombineTerms(
                            left,
                            new IdentityTerm(identity.Identity),
                            right);
                    }
                    // "abc +" ==> "+ abc"
                    else
                    {
                        runnerContext.CurrentTerm = CombineTerms(
                            new IdentityTerm(identity.Identity),
                            runnerContext.CurrentTerm);
                    }
                }
                else
                {
                    // Will not swap
                    runnerContext.CurrentTerm = CombineTerms(
                        runnerContext.CurrentTerm,
                        new IdentityTerm(identity.Identity));
                }

                // Pre marking RTL
                runnerContext.ApplyNextAssociative = terms[0].Associative;
            }
            // "abc def"
            else
            {
                runnerContext.CurrentTerm = CombineTerms(
                    runnerContext.CurrentTerm,
                    new IdentityTerm(identity.Identity));
            }
            return ParseRunnerResult.Empty(ApplyingRunner.Instance);
        }

        public static LeaveScopeResults LeaveOneImplicitScope(ParseRunnerContext runnerContext)
        {
            // Get last parenthesis scope:
            if (runnerContext.TryPopScope(out var scope))
            {
                // Did this scope have parenthesis?
                if (scope.ParenthesisPair is ParenthesisPair scopeParenthesisPair)
                {
                    throw new InvalidOperationException(
                        $"Unmatched parenthesis: Opened={scopeParenthesisPair.Open}");
                }
                // Implicit (RTL) scope:
                else
                {
                    // Make term hiding:
                    // because invalid deconstruction ApplyTerm for next token iteration.
                    var hideTerm = HideTerm(runnerContext.CurrentTerm);

                    // Combine it implicitly.
                    runnerContext.CurrentTerm = CombineTerms(
                        scope.SavedTerm,
                        hideTerm);

                    // Reset precedence, because finished a scope.
                    runnerContext.CurrentPrecedence = null;

                    // Leave Implicit scope.
                    return LeaveScopeResults.Implicitly;
                }
            }
            else
            {
                // Make term hiding:
                // because invalid deconstruction ApplyTerm for next token iteration.
                runnerContext.CurrentTerm = HideTerm(runnerContext.CurrentTerm);

                // Matching scope didn't find
                return LeaveScopeResults.None;
            }
        }

        public static LeaveScopeResults LeaveOneScope(
            ParseRunnerContext runnerContext, ParenthesisPair parenthesisPair)
        {
            // Get last parenthesis scope:
            if (runnerContext.TryPopScope(out var scope))
            {
                if (scope.ParenthesisPair is ParenthesisPair scopeParenthesisPair)
                {
                    // Is parenthesis not matching
                    if (scopeParenthesisPair.Close != parenthesisPair.Close)
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesisPair.Close}, Opened={scopeParenthesisPair.Open}");
                    }

                    // Make term hiding:
                    // because invalid deconstruction ApplyTerm for next token iteration.
                    var hideTerm = HideTerm(runnerContext.CurrentTerm);

                    // Parenthesis scope:
                    runnerContext.CurrentTerm = CombineTerms(
                        scope.SavedTerm,
                        hideTerm);

                    // Reset precedence, because finished a scope.
                    runnerContext.CurrentPrecedence = null;

                    // Matched scope
                    return LeaveScopeResults.Explicitly;
                }
                // Implicit (RTL) scope:
                else
                {
                    // Make term hiding:
                    // because invalid deconstruction ApplyTerm for next token iteration.
                    var hideTerm = HideTerm(runnerContext.CurrentTerm);

                    // Combine it implicitly.
                    runnerContext.CurrentTerm = CombineTerms(
                        scope.SavedTerm,
                        hideTerm);

                    // Reset precedence, because finished a scope.
                    runnerContext.CurrentPrecedence = null;

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
    }
}
