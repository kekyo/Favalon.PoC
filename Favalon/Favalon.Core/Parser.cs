using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Globalization;

namespace Favalon
{
    public static class Parser
    {
        private static ConstantTerm GetNumericConstant(string value, Token? preSign)
        {
            var sign = preSign switch
            {
                OperatorToken("-") => -1,
                _ => 1,
            };
            var intValue = int.Parse(value, CultureInfo.InvariantCulture) * sign;
            return new ConstantTerm(intValue);
        }

        public static IEnumerable<Term> EnumerableTerms(IEnumerable<Token> tokens)
        {
            // Special parser features:
            // 1. Will have capablility for translating numerics before unary signed operator (+/-).
            // 2. Will make (applicable) function from all operator tokens by transposing.

            Token? lastToken = null;
            OperatorToken? lastSignToken = null;
            Term? rootTerm = null;
            var stack = new Stack<Term?>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case OperatorToken("+"):
                    case OperatorToken("-"):
                        var signToken = (OperatorToken)token;
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
                                        // auto transposed, it's single binary operator +/-
                                        rootTerm = new ApplyTerm(
                                            new IdentityTerm(signToken.Symbol),
                                            rootTerm);
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(signToken.Symbol);
                                        break;
                                }
                                break;
                        }
                        break;

                    case OperatorToken("(") _:
                        stack.Push(rootTerm);
                        rootTerm = null;
                        lastSignToken = null;
                        break;

                    case OperatorToken(")") _:
                        var lastTerm = stack.Pop();
                        if ((rootTerm != null) && (lastTerm != null))
                        {
                            rootTerm = new ApplyTerm(lastTerm, rootTerm);
                        }
                        else if (lastTerm != null)
                        {
                            rootTerm = lastTerm;
                        }
                        lastSignToken = null;
                        break;

                    case OperatorToken operatorToken:
                        switch (rootTerm)
                        {
                            case Term _:
                                // auto transposed because it's a operator
                                rootTerm = new ApplyTerm(
                                    new IdentityTerm(operatorToken.Symbol),
                                    rootTerm);
                                break;
                            default:
                                rootTerm = new IdentityTerm(operatorToken.Symbol);
                                break;
                        }
                        lastSignToken = null;
                        break;

                    case IdentityToken identityToken:
                        switch (rootTerm)
                        {
                            case Term _:
                                rootTerm = new ApplyTerm(rootTerm, new IdentityTerm(identityToken.Identity));
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
                            case OperatorToken("+"):
                            case OperatorToken("-"):
                                switch (rootTerm)
                                {
                                    case Term _:
                                        // auto transposed, it's single binary operator +/-
                                        rootTerm = new ApplyTerm(
                                            new IdentityTerm(lastSignToken.Symbol),
                                            rootTerm);
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(lastSignToken.Symbol);
                                        break;
                                }
                                lastSignToken = null;
                                break;
                        }
                        break;
                }

                lastToken = token;
            }

            if (rootTerm != null)
            {
                yield return rootTerm;
            }
        }
    }
}
