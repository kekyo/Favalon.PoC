using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Globalization;
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

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Term? HideTerm(Term? term) =>
            term is ApplyTerm apply ? new HidedApplyTerm(apply) : term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ConstantTerm GetNumericConstant(string value, NumericalSignes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);

        public static ParseRunnerResult RunIdentity(ParseRunnerContext runnerContext, IdentityToken identity)
        {
            if (runnerContext.CurrentContext.LookupBoundTerms(identity.Identity) is BoundTermInformation[] terms)
            {
                // This term is given precedence
                if (terms[0].Precedence is BoundTermPrecedences termPrecedence)
                {
                    // Not first time
                    if (runnerContext.CurrentPrecedence is BoundTermPrecedences precedence)
                    {
                        // Greater than current precedence
                        if (termPrecedence > precedence)
                        {
                            // (Transpose SavedTerm)
                            if (runnerContext.CurrentTerm is ApplyTerm(Term left, Term right))
                            {
                                // Swap (unapply) and begin new implicit scope
                                // "+ abc def   *" ==> "+ abc (def   *"
                                //  left  right id      left  (right id
                                runnerContext.SetTerm(left);
                                runnerContext.PushScope();

                                // Set first term from right
                                runnerContext.SetTerm(right);
                            }
                            else
                            {
                                // Begin new implicit scope
                                // "+ *" ==> "+ (*"
                                runnerContext.PushScope();
                            }

                            // Update precedence
                            runnerContext.SetPrecedence(termPrecedence);
                        }
                        // Lesser than current precedence
                        else if (termPrecedence < precedence)
                        {
                            // Leave one implicit scope 
                            // And disable deconstruct last ApplyTerm if didn't scope out
                            LeaveOneImplicitScope(runnerContext);

                            // Update precedence
                            runnerContext.SetPrecedence(termPrecedence);
                        }
                    }
                    else
                    {
                        // Update precedence
                        runnerContext.SetPrecedence(termPrecedence);
                    }
                }

                // Is this identity infix?
                if (terms[0].Notation == BoundTermNotations.Infix)
                {
                    // "abc def +" ==> "abc + def"
                    // (Dodge SavedTerm)
                    if (runnerContext.CurrentTerm is ApplyTerm(Term left, Term right))
                    {
                        runnerContext.SetTerm(left);
                        runnerContext.CombineAfter(terms[0].Term);
                        runnerContext.CombineAfter(right);
                    }
                    // "abc +" ==> "+ abc"
                    else
                    {
                        runnerContext.CombineBefore(terms[0].Term);
                    }
                }
                else
                {
                    // Will not swap
                    runnerContext.CombineAfter(terms[0].Term);
                }

                // Pre marking RTL
                runnerContext.ApplyNextAssociative = terms[0].Associative;
            }
            // "abc def"
            else
            {
                runnerContext.CombineAfter(new IdentityTerm(identity.Identity));
            }
            return ParseRunnerResult.Empty(ApplyingRunner.Instance);
        }

        public static LeaveScopeResults LeaveOneImplicitScope(ParseRunnerContext runnerContext)
        {
            // Get last parenthesis scope:
            if (runnerContext.TryPopScope(out var spp))
            {
                // Did this scope have parenthesis?
                if (spp is ParenthesisPair scopeParenthesisPair)
                {
                    throw new InvalidOperationException(
                        $"Unmatched parenthesis: Opened={scopeParenthesisPair.Open}");
                }
                // Implicit (RTL) scope:
                else
                {
                    // Leave Implicit scope.
                    return LeaveScopeResults.Implicitly;
                }
            }
            else
            {
                // Make term hiding:
                // because invalid deconstruction ApplyTerm for next token iteration.
                runnerContext.MakeHidedApplyTerm();

                // Matching scope didn't find
                return LeaveScopeResults.None;
            }
        }

        public static LeaveScopeResults LeaveOneScope(
            ParseRunnerContext runnerContext, ParenthesisPair parenthesisPair)
        {
            // Get last scope:
            if (runnerContext.TryPopScope(out var spp))
            {
                // Did this scope have parenthesis?
                if (spp is ParenthesisPair scopeParenthesisPair)
                {
                    // Is parenthesis not matching?
                    if (scopeParenthesisPair.Close != parenthesisPair.Close)
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesisPair.Close}, Opened={scopeParenthesisPair.Open}");
                    }

                    // Matched scope
                    return LeaveScopeResults.Explicitly;
                }
                // Implicit (RTL) scope:
                else
                {
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
