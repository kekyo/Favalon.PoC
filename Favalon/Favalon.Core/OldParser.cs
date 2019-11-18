using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Globalization;

namespace Favalon
{
    public static class OldParser
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

        public static IEnumerable<Term> EnumerableTerms(IEnumerable<Token> tokens)
        {
            // Special parser features:
            // * Will have capablility for translating numerics before unary signed operator (+/-).

            Token? lastToken = null;
            NumericalSignToken? lastSignToken = null;
            Term? rootTerm = null;
            var stack = new Stack<Term?>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case NumericalSignToken('+'):
                    case NumericalSignToken('-'):
                        var signToken = (NumericalSignToken)token;
                        switch (lastToken)
                        {
                            case WhiteSpaceToken _:
                            case null:
                                lastSignToken = signToken;
                                break;
                            default:
                                switch (rootTerm)
                                {
                                    case Term _:
                                        rootTerm = new ApplyTerm(
                                            rootTerm,
                                            new IdentityTerm(signToken.Symbol.ToString()));
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(signToken.Symbol.ToString());
                                        break;
                                }
                                break;
                        }
                        break;

                    case IdentityToken("(") _:
                        stack.Push(rootTerm);
                        rootTerm = null;
                        lastSignToken = null;
                        break;

                    case IdentityToken(")") _:
                        var lastTerm = stack.Pop();
                        if (lastTerm != null)
                        {
                            if (rootTerm != null)
                            {
                                rootTerm = new ApplyTerm(lastTerm, rootTerm);
                            }
                            else
                            {
                                rootTerm = lastTerm;
                            }
                        }
                        stack.Push(rootTerm);
                        rootTerm = null;
                        lastSignToken = null;
                        break;

                    case IdentityToken identityToken:
                        switch (rootTerm)
                        {
                            case Term _:
                                rootTerm = new ApplyTerm(
                                    rootTerm,
                                    new IdentityTerm(identityToken.Identity));
                                break;
                            default:
                                rootTerm = new IdentityTerm(identityToken.Identity);
                                break;
                        }
                        lastSignToken = null;
                        break;

                    case NumericToken numericToken:
                        switch (rootTerm)
                        {
                            case Term _:
                                rootTerm = new ApplyTerm(
                                    rootTerm,
                                    GetNumericConstant(numericToken.Value, lastSignToken));
                                break;
                            default:
                                rootTerm = GetNumericConstant(numericToken.Value, lastSignToken);
                                break;
                        }
                        lastSignToken = null;
                        break;

                    case WhiteSpaceToken _:
                        switch (lastSignToken)
                        {
                            case NumericalSignToken('+'):
                            case NumericalSignToken('-'):
                                switch (rootTerm)
                                {
                                    case Term _:
                                        rootTerm = new ApplyTerm(
                                            rootTerm,
                                            new IdentityTerm(lastSignToken.Symbol.ToString()));
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(lastSignToken.Symbol.ToString());
                                        break;
                                }
                                lastSignToken = null;
                                break;
                        }
                        break;
                }

                lastToken = token;
            }

            // Final consuming for left terms.
            while (stack.Count >= 1)
            {
                var leftTerm = stack.Pop();
                if (leftTerm != null)
                {
                    if (rootTerm != null)
                    {
                        rootTerm = new ApplyTerm(leftTerm, rootTerm);
                    }
                    else
                    {
                        rootTerm = leftTerm;
                    }
                }
            }

            if (rootTerm != null)
            {
                yield return rootTerm;
            }
        }
    }
}
