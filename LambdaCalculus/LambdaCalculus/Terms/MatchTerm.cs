using Favalon.Contexts;
using Favalon.Terms.Algebric;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class MatchTerm : HigherOrderHoldTerm, IApplicable
    {
        public readonly Term[] Matchers;

        internal MatchTerm(Term[] matchers, Term higherOrder) :
            base(higherOrder) =>
            this.Matchers = matchers;

        public override Term Infer(InferContext context)
        {
            // Best effort infer procedure.

            var matchers = this.Matchers.
                Select(entry =>
                {
                    if (entry is PairTerm(UnspecifiedTerm match, Term body))
                    {
                        var body_ = body.Infer(context);
                        return object.ReferenceEquals(body_, body) ?
                            entry :
                            new PairTerm(match, body_);
                    }
                    else
                    {
                        return entry.Infer(context);
                    }
                }).
                ToArray();

            var higherOrder = this.HigherOrder.Infer(context);

            foreach (var entry in matchers)
            {
                if (entry is PairTerm(_, Term body))
                {
                    context.Unify(body.HigherOrder, higherOrder);
                }
            }

            return
                object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                matchers.Zip(this.Matchers, object.ReferenceEquals).All(r => r) ?
                    this :
                    new MatchTerm(matchers, higherOrder);
        }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint)
        {
            // Strict infer procedure.

            var higherOrder = this.HigherOrder.Infer(context);
            context.Unify(higherOrder, higherOrderHint);

            var matchers = this.Matchers.
                Select(entry =>
                {
                    if (entry is PairTerm(UnspecifiedTerm match, Term body))
                    {
                        var body_ = body.Infer(context);
                        return object.ReferenceEquals(body_, body) ?
                            entry :
                            new PairTerm(match, body_);
                    }
                    else
                    {
                        // Infer entry but cannot derives higher order hint.
                        return entry.Infer(context);
                    }
                }).
                ToArray();

            foreach (var entry in matchers)
            {
                if (entry is PairTerm(_, Term body))
                {
                    context.Unify(body.HigherOrder, higherOrderHint);
                }
            }

            return
                object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                matchers.Zip(this.Matchers, object.ReferenceEquals).All(r => r) ?
                    this :
                    new MatchTerm(matchers, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            // Best effort fixup procedure.

            var matchers = this.Matchers.
                Select(entry => entry.Fixup(context)).
                ToArray();

            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                matchers.Zip(this.Matchers, object.ReferenceEquals).All(r => r) ?
                    this :
                    new MatchTerm(matchers, higherOrder);
        }

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint)
        {
            // Strict fixup procedure.

            var higherOrder = this.HigherOrder.Fixup(context);

            var matchers = this.Matchers.
                Select(entry =>
                {
                    if (entry is PairTerm(UnspecifiedTerm match, Term body))
                    {
                        var body_ = body.Fixup(context);
                        return object.ReferenceEquals(body_, body) ?
                            entry :
                            new PairTerm(match, body_);
                    }
                    else
                    {
                        // Infer entry but cannot derives higher order hint.
                        return entry.Fixup(context);
                    }
                }).
                ToArray();

            return
                object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                matchers.Zip(this.Matchers, object.ReferenceEquals).All(r => r) ?
                    this :
                    new MatchTerm(matchers, higherOrder);
        }

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
        {
            var argument_ = argument.Reduce(context);

            var reducedMatches = new List<Term>();
            foreach (var entry in this.Matchers)
            {
                // _ => ...
                if (entry is PairTerm(Term match, Term body))
                {
                    if (match is UnspecifiedTerm)
                    {
                        return body.Reduce(context);
                    }

                    var reducedMatch = match.Reduce(context);
                    if (reducedMatch.Equals((Term)argument_))   // TODO: Recursive matcher
                    {
                        return body.Reduce(context);
                    }

                    reducedMatches.Add(reducedMatch);
                }
                else
                {
                    // TODO: Will cause syntax error for invalid AST format... ?
                }
            }

            // Didn't match for all: reconstruct MatchTerm by reduced matchers.

            var matchers = reducedMatches.Zip(
                this.Matchers.Cast<PairTerm>(),
                (match, entry) => object.ReferenceEquals(match, entry.Lhs) ?
                    entry :
                    new PairTerm(match, entry.Rhs)).
                ToArray();
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                matchers.Zip(this.Matchers, object.ReferenceEquals).All(r => r) ?
                    null :
                    new MatchTerm(matchers, higherOrder);
        }

        public override bool Equals(Term? other) =>
            other is MatchTerm rhs ? this.Matchers.SequenceEqual(rhs.Matchers) : false;

        public override int GetHashCode() =>
            this.Matchers.Aggregate(0, (agg, pair) => agg ^ pair.GetHashCode());
    }
}
