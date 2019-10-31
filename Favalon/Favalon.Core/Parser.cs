using Favalon.Terms;
using Favalon.Tokens;
using System;
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
                IdentityToken("-") => -1,
                _ => 1,
            };
            var intValue = int.Parse(value, CultureInfo.InvariantCulture) * sign;
            return new ConstantTerm(intValue);
        }

        public static IEnumerable<Term> EnumerableTerms(IEnumerable<Token> tokens)
        {
            Token? lastToken = null;
            IdentityToken? lastSignToken = null;
            Term? rootTerm = null;
            var stack = new Stack<Term?>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case IdentityToken("+"):
                    case IdentityToken("-"):
                        switch (lastToken)
                        {
                            case WhiteSpaceToken _:
                            case null:
                                lastSignToken = (IdentityToken)token;
                                break;
                            default:
                                switch (rootTerm)
                                {
                                    case Term _:
                                        rootTerm = new ApplyTerm(rootTerm, new IdentityTerm(((IdentityToken)token).Identity));
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(((IdentityToken)token).Identity);
                                        break;
                                }
                                break;
                        }
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
                                rootTerm = new ApplyTerm(rootTerm, GetNumericConstant(numericToken.Value, lastSignToken));
                                break;
                            default:
                                rootTerm = GetNumericConstant(numericToken.Value, lastSignToken);
                                break;
                        }
                        lastSignToken = null;
                        break;

                    case BeginBracketToken _:
                        stack.Push(rootTerm);
                        rootTerm = null;
                        lastSignToken = null;
                        break;

                    case EndBracketToken _:
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

                    case WhiteSpaceToken _:
                        switch (lastSignToken)
                        {
                            case IdentityToken("+"):
                            case IdentityToken("-"):
                                switch (rootTerm)
                                {
                                    case Term _:
                                        rootTerm = new ApplyTerm(rootTerm, new IdentityTerm(lastSignToken.Identity));
                                        break;
                                    default:
                                        rootTerm = new IdentityTerm(lastSignToken.Identity);
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
