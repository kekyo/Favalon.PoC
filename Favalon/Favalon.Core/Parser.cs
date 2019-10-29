using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Favalon
{
    public static class Parser
    {
        private static ConstantTerm GetNumericConstant(string value)
        {
            var intValue = int.Parse(value, CultureInfo.InvariantCulture);
            return new ConstantTerm(intValue);
        }

        public static IEnumerable<Term> EnumerableTerms(IEnumerable<Token> tokens)
        {
            Term? root = null;
            var stack = new Stack<Term?>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case IdentityToken identityToken:
                        switch (root)
                        {
                            case Term _:
                                root = new ApplyTerm(root, new IdentityTerm(identityToken.Identity));
                                break;
                            default:
                                root = new IdentityTerm(identityToken.Identity);
                                break;
                        }
                        break;
                    case NumericToken numericToken:
                        switch (root)
                        {
                            case Term _:
                                root = new ApplyTerm(root, GetNumericConstant(numericToken.Value));
                                break;
                            default:
                                root = GetNumericConstant(numericToken.Value);
                                break;
                        }
                        break;
                    case BeginBracketToken _:
                        stack.Push(root);
                        root = null;
                        break;
                    case EndBracketToken _:
                        var lastTerm = stack.Pop();
                        if ((root != null) && (lastTerm != null))
                        {
                            root = new ApplyTerm(lastTerm, root);
                        }
                        else if (lastTerm != null)
                        {
                            root = lastTerm;
                        }
                        break;
                }
            }

            if (root != null)
            {
                yield return root;
            }
        }
    }
}
