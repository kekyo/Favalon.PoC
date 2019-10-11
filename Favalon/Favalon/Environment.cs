using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Environment
    {
        private sealed class OverallScope
        {
            private int placeholderIndex;

            public int AssignIndex() =>
                placeholderIndex++;
        }

        private readonly OverallScope overallScope;
        private readonly Dictionary<string, Term> boundTerms;

        private Environment(OverallScope overallScope, Dictionary<string, Term> boundTerms)
        {
            this.overallScope = overallScope;
            this.boundTerms = boundTerms;
        }

        public IReadOnlyDictionary<string, Term> BoundTerms =>
            boundTerms;

        public Environment Bind(string name, Term body) =>
            new Environment(
                overallScope,
                new Dictionary<string, Term>(boundTerms)
                {
                    { name, body }
                });

        internal Term? Lookup(string name) =>
            boundTerms.TryGetValue(name, out var body) ? body : null;

        internal Placeholder CreatePlaceholder(Term higherOrder) =>
            new Placeholder(overallScope.AssignIndex(), higherOrder);

        public Term Infer(Term term)
        {
            var inferred = term.VisitInfer(this);
            return inferred;
        }

        public static Environment Create()
        {
            var overallScope = new OverallScope();
            var boundTerms = new Dictionary<string, Term>();
            var environment = new Environment(overallScope, boundTerms);

            var t0 = environment.CreatePlaceholder(Unspecified.Instance);
            var t1 = environment.CreatePlaceholder(Unspecified.Instance);
            boundTerms.Add(
                "->",
                new Variable("->",
                    new Function(t0, new Function(t1, new Function(t0, t1)))));

            return environment;
        }
    }
}
