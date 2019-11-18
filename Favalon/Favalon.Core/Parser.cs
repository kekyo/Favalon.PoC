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
            ConstantTerm GetNumericConstant(string value, Signs preSign) =>
                new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);

            Term TermOrApply(Term? left, Term? right) =>
                left is Term ?
                    right is Term ?
                        new ApplyTerm(left, right) :
                        left :
                    right!;

            Term? currentTerm = null;
            Token? lastToken = null;
            var stack = new Stack<OneOfTermInformation>();
            NumericalSignToken? preSign = null;
            var separatedSign = false;

            foreach (var token in tokens)
            {
                if (token is NumericalSignToken numericalSign)
                {
                    preSign = numericalSign;
                    separatedSign = (lastToken is WhiteSpaceToken) || (lastToken == null);
                }
                else
                {
                    switch (token)
                    {
                        case WhiteSpaceToken _:
                            if (preSign is NumericalSignToken sign1)
                            {
                                currentTerm = TermOrApply(
                                    currentTerm,
                                    new IdentityTerm(sign1.ToString()));
                            }
                            break;

                        case NumericToken numeric:
                            if (preSign is NumericalSignToken sign2)
                            {
                                // abc -123
                                if (separatedSign)
                                {
                                    var numericTerm = GetNumericConstant(
                                        numeric.Value,
                                        sign2.Sign);
                                    currentTerm = TermOrApply(
                                        currentTerm,
                                        numericTerm);
                                }
                                // abc-123
                                else
                                {
                                    // Examined binary op
                                    currentTerm = TermOrApply(
                                        currentTerm,
                                        new IdentityTerm(sign2.ToString()));
                                    var numericTerm = GetNumericConstant(
                                        numeric.Value,
                                        Signs.Plus);
                                    currentTerm = TermOrApply(
                                        currentTerm,
                                        numericTerm);
                                }
                            }
                            // abc 123
                            else
                            {
                                var numericTerm = GetNumericConstant(
                                    numeric.Value,
                                    Signs.Plus);
                                currentTerm = TermOrApply(
                                    currentTerm,
                                    numericTerm);
                            }
                            break;

                        case IdentityToken identity:
                            currentTerm = TermOrApply(
                                currentTerm,
                                new IdentityTerm(identity.Identity));
                            break;

                        case OpenParenthesisToken parenthesis:
                            stack.Push(new OneOfTermInformation(
                                currentTerm,
                                Characters.IsOpenParenthesis(parenthesis.Symbol)!.Value.Close));
                            currentTerm = null;
                            break;

                        case CloseParenthesisToken parenthesis:
                            if (stack.Count >= 1)
                            {
                                var oneOfTermInformation = stack.Pop();
                                if (oneOfTermInformation.Close == parenthesis.Symbol)
                                {
                                    if (oneOfTermInformation.Term != null)
                                    {
                                        currentTerm = TermOrApply(
                                            oneOfTermInformation.Term,
                                            currentTerm);
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

                    preSign = null;
                    separatedSign = false;
                }

                lastToken = token;
            }

            if (stack.Count >= 1)
            {
                // Lack for close parenthesis.
                throw new InvalidOperationException();
            }

            if (currentTerm != null)
            {
                yield return currentTerm;
            }
        }
    }
}
