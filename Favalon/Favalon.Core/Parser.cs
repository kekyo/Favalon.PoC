using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Favalon
{
    internal static class Parser
    {
        private static ConstantTerm GetNumericConstant(string value, Token? preSign)
        {
            var sign = preSign switch
            {
                NumericalSignToken('-') => -1,
                _ => 1,
            };
            var intValue = int.Parse(value, CultureInfo.InvariantCulture) * sign;
            return new ConstantTerm(intValue);
        }

        private static Term? Parse(Environment environment, IEnumerable<Token> tokens)
        {
            Term? lastTerm = null;
            var stack = new Stack<Term?>();

            foreach (var token in tokens)
            {
                Term term;

                switch (token)
                {
                    case IdentityToken identity:
                        term = new IdentityTerm(identity.Identity);
                        if (lastTerm != null)
                        {
                            term = new ApplyTerm(lastTerm, term);
                        }
                        break;
                    case OpenParenthesisToken parenthesis:

                        break;
                    default:
                        throw new InvalidOperationException();
                }

                lastTerm = term;
            }

            return lastTerm;
        }

        public static IEnumerable<Term> EnumerableTerms(Environment environment, IEnumerable<Token> tokens)
        {
            if (Parse(environment, tokens) is Term term)
            {
                yield return term;
            }
        }
    }
}
