using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;

namespace Favalon
{
    public static class Parser
    {
        public static IEnumerable<Term> EnumerableTerms(IEnumerable<Token> tokens)
        {
            Term? root = null;
            var stack = new Stack<Term?>();
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case IdentityToken identityToken:
                        var identityTerm = new IdentityTerm(identityToken.Identity);
                        if (root != null)
                        {
                            root = new ApplyTerm(root, identityTerm);
                        }
                        else
                        {
                            root = identityTerm;
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
