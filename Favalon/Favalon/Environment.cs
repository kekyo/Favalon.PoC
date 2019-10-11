using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Environment
    {
        private readonly Dictionary<string, Term> boundTerms;

        private Environment(Dictionary<string, Term> boundTerms) =>
            this.boundTerms = boundTerms;

        public Environment Bind(string name, Term body) =>
            new Environment(
                new Dictionary<string, Term>(boundTerms)
                {
                    { name, body }
                });

        internal Term? Lookup(string name) =>
            boundTerms.TryGetValue(name, out var body) ? body : null;

        public Term Infer(Term term)
        {
            var inferred = term.VisitInfer(this);
            return inferred;
        }

        public static Environment Create() =>
            new Environment(new Dictionary<string, Term>());
    }
}
