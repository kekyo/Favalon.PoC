using Favalon.Internals;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    public sealed class Checker
    {
        private readonly Dictionary<string, Term> boundTerms =
            new Dictionary<string, Term>();

        public void Add(string variable, Term term) =>
            boundTerms.Add(variable, term);

        public IEnumerable<Term> Infer(IEnumerable<Term> terms)
        {
            return terms;
        }
    }
}
