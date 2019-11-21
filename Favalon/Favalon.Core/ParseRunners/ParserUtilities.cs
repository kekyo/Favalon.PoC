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

        public static bool LeaveScope(
            ParseRunnerContext context, ParenthesisPair? parenthesisPair)
        {
            while (context.Scopes.Count >= 1)
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
    }
}
