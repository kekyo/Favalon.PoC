using Favalon.Internal;
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

        private struct OneOfTermInformation
        {
            public readonly Term? Term;
            public readonly char Close;

            public OneOfTermInformation(Term? term, char close)
            {
                this.Term = term;
                this.Close = close;
            }
        }

        public static IEnumerable<Term> EnumerableTerms(
            Environment environment,
            IEnumerable<Token> tokens)
        {
            Term? lastTerm = null;
            var stack = new Stack<OneOfTermInformation>();

            foreach (var token in tokens)
            {
                switch (token)
                {
                    case IdentityToken identity:
                        Term newTerm = new IdentityTerm(identity.Identity);
                        if (lastTerm != null)
                        {
                            lastTerm = new ApplyTerm(lastTerm, newTerm);
                        }
                        else
                        {
                            lastTerm = newTerm;
                        }
                        break;

                    case OpenParenthesisToken parenthesis:
                        stack.Push(new OneOfTermInformation(
                            lastTerm,
                            Characters.IsOpenParenthesis(parenthesis.Symbol)!.Value.Close));
                        lastTerm = null;
                        break;

                    case CloseParenthesisToken parenthesis:
                        if (stack.Count >= 1)
                        {
                            var oneOfTermInformation = stack.Pop();
                            if (oneOfTermInformation.Close == parenthesis.Symbol)
                            {
                                if (oneOfTermInformation.Term != null)
                                {
                                    if (lastTerm != null)
                                    {
                                        lastTerm = new ApplyTerm(oneOfTermInformation.Term, lastTerm);
                                    }
                                    else
                                    {
                                        lastTerm = oneOfTermInformation.Term;
                                    }
                                }
                            }
                            else
                            {
                                // Invalid parenthesis pair.
                                throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            // Lack for open parenthesis.
                            throw new InvalidOperationException();
                        }
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            if (stack.Count >= 1)
            {
                // Lack for close parenthesis.
                throw new InvalidOperationException();
            }

            if (lastTerm != null)
            {
                yield return lastTerm;
            }
        }
    }
}
