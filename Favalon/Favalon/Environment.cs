using Favalon.Expressions;
using Favalon.Terms;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

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

        private static readonly IReadOnlyList<Term> emptyTerms = new Term[0];
        private static readonly ImmutableDictionary<string, ImmutableList<Term>> defaultBoundTerms;

        static Environment()
        {
            defaultBoundTerms = Utilities.EmptyBoundTerms.
                AddAssemblyMembers(typeof(object).GetTypeInfo().Assembly).
                AddExecutables(@"C:\Program Files\Git\usr\bin", "*.exe");
        }

        private readonly OverallScope overallScope;
        private readonly ImmutableDictionary<string, ImmutableList<Term>> boundTerms;

        private Environment(OverallScope overallScope, ImmutableDictionary<string, ImmutableList<Term>> boundTerms)
        {
            this.overallScope = overallScope;
            this.boundTerms = boundTerms;
        }

        public Environment Bind(string name, Term body) =>
            new Environment(
                overallScope,
                boundTerms.TryGetValue(name, out var terms) ?
                    boundTerms.SetItem(name, terms.Add(body)) :
                    boundTerms.Add(name, ImmutableList<Term>.Empty.Add(body)));

        internal IReadOnlyList<Term> Lookup(string name) =>
            boundTerms.TryGetValue(name, out var body) ? body : emptyTerms;

        internal Placeholder CreatePlaceholder(Term higherOrder) =>
            new Placeholder(overallScope.AssignIndex(), higherOrder);

        public Expression Infer(Term term) =>
            term.Visit(this);

        public static Environment Create() =>
            new Environment(new OverallScope(), defaultBoundTerms);
    }
}
