using Favalon.AlgebricData;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed class MatchTerm : Term
    {
        public readonly Term Term;
        public readonly PairTerm[] Matchers;

        internal MatchTerm(Term term, PairTerm[] matchers, Term higherOrder)
        {
            this.Term = term;
            this.Matchers = matchers;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(ReduceContext context)
        {
            var term = this.Term.Reduce(context);

            var skipMatches = new List<Term>();
            foreach (var pair in this.Matchers)
            {
                // _ => ...
                if (pair.Lhs is UnspecifiedTerm)
                {
                    return pair.Rhs.Reduce(context);
                }

                var match = pair.Lhs.Reduce(context);
                if (match.Equals(term))
                {
                    return pair.Rhs.Reduce(context);
                }
                skipMatches.Add(match);
            }

            return new MatchTerm(
                term,
                skipMatches.Zip(
                    this.Matchers.Select(pair => pair.Rhs.Reduce(context)),
                    (match, body) => new PairTerm(match, body)).
                    ToArray(),
                this.HigherOrder.Reduce(context));
        }

        public override Term Infer(InferContext context)
        {
            var term = this.Term.Infer(context);

            var matchers = this.Matchers.
                Select(pair => pair.Lhs is UnspecifiedTerm ?
                    new PairTerm(UnspecifiedTerm.Instance, pair.Rhs.Infer(context)) :
                    (PairTerm)pair.Infer(context)).
                ToArray();

            var higherOrder = this.HigherOrder.Infer(context);

            foreach (var pair in matchers)
            {
                // TODO: Maybe ignore if term will be matched by higher orders.
                //context.Unify(pair.Lhs.HigherOrder, term.HigherOrder);

                context.Unify(pair.Rhs.HigherOrder, higherOrder);
            }

            return new MatchTerm(term, matchers, higherOrder);
        }

        public override Term Fixup(FixupContext context) =>
            new MatchTerm(
                this.Term.Fixup(context),
                this.Matchers.Select(pair => (PairTerm)pair.Fixup(context)).ToArray(),
                this.HigherOrder.Fixup(context));

        public override bool Equals(Term? other) =>
            other is MatchTerm rhs ?
                (this.Term.Equals(rhs.Term) && this.Matchers.SequenceEqual(rhs.Matchers)) :
                false;

        public override int GetHashCode() =>
            this.Matchers.Aggregate(this.Term.GetHashCode(), (agg, pair) => agg ^ pair.GetHashCode());
    }
}
