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
            var enumerator = tokens.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    if (enumerator.Current is IdentityToken identityToken)
                    {
                        Term term = new IdentityTerm(identityToken.Identity);
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current is IdentityToken token)
                            {
                                term = new ApplyTerm(term, new IdentityTerm(token.Identity));
                            }
                            else
                            {
                                // TODO: error invalid token sequence?
                            }
                        }

                        yield return term;
                    }
                    else
                    {
                        // TODO: error invalid token sequence?
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
