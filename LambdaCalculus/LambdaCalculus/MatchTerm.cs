using Favalon.AlgebricData;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed class MatchTerm : Term, IApplicable
    {
        public readonly PairTerm[] Matchers;

        internal MatchTerm(PairTerm[] matchers, Term higherOrder)
        {
            this.Matchers = matchers;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            var matchers = this.Matchers.
                Select(pair => pair.Lhs is UnspecifiedTerm ?
                    new PairTerm(UnspecifiedTerm.Instance, pair.Rhs.Infer(context)) :
                    (PairTerm)pair.Infer(context)).
                ToArray();

            var higherOrder = this.HigherOrder.Infer(context);

            foreach (var pair in matchers)
            {
                context.Unify(pair.Rhs.HigherOrder, higherOrder);
            }

            return new MatchTerm(matchers, higherOrder);
        }

        public override Term Fixup(FixupContext context) =>
            new MatchTerm(
                this.Matchers.Select(pair => (PairTerm)pair.Fixup(context)).ToArray(),
                this.HigherOrder.Fixup(context));

        public override Term Reduce(ReduceContext context) =>
            this;

        internal static Term Reduce(ReduceContext context, Term term, PairTerm[] matchers, Term higherOrder)
        {
            var term_ = term.Reduce(context);

            var reducedMatches = new List<Term>();
            foreach (var pair in matchers)
            {
                // _ => ...
                if (pair.Lhs is UnspecifiedTerm)
                {
                    return pair.Rhs.Reduce(context);
                }

                var reducedMatch = pair.Lhs.Reduce(context);
                if (reducedMatch.Equals(term_))   // TODO: Recursive matcher
                {
                    return pair.Rhs.Reduce(context);
                }
                reducedMatches.Add(reducedMatch);
            }

            return new MatchTerm(
                reducedMatches.Zip(
                    matchers.Select(pair => pair.Rhs),
                    (match, body) => new PairTerm(match, body)).
                    ToArray(),
                higherOrder.Reduce(context));
        }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            Reduce(context, rhs, this.Matchers, this.HigherOrder);

        public override bool Equals(Term? other) =>
            other is MatchTerm rhs ? this.Matchers.SequenceEqual(rhs.Matchers) : false;

        public override int GetHashCode() =>
            this.Matchers.Aggregate(0, (agg, pair) => agg ^ pair.GetHashCode());
    }
}
