using Favalon.AlgebricData;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Types
{
    public sealed class DiscriminatedUnionTerm : HigherOrderHoldTerm
    {
        public readonly PairTerm[] Constructors;

        internal DiscriminatedUnionTerm(IEnumerable<PairTerm> constructors, Term higherOrder) :
            base(higherOrder) =>
            this.Constructors = constructors.ToArray();

        public override Term Infer(InferContext context)
        {
            var term = new DiscriminatedUnionTerm(
                this.Constructors.Select(pair => (PairTerm)pair.Infer(context)),
                this.HigherOrder.Infer(context));

            foreach (var constructor in term.Constructors)
            {
                context.Unify(
                    constructor.Rhs.HigherOrder is LambdaTerm lambda ?
                        lambda.Body :
                        constructor.Rhs.HigherOrder,
                    term);
            }

            return term;
        }

        public override Term Fixup(FixupContext context) =>
            new DiscriminatedUnionTerm(
                this.Constructors.Select(pair => (PairTerm)pair.Fixup(context)),
                this.HigherOrder.Fixup(context));

        public override Term Reduce(ReduceContext context)
        {
            var term = new DiscriminatedUnionTerm(
                this.Constructors.Select(pair => (PairTerm)pair.Reduce(context)),
                this.HigherOrder.Reduce(context));

            foreach (var constructor in term.Constructors)
            {
                if (constructor.Lhs is IdentityTerm identity)
                {
                    context.SetBoundTerm(identity.Identity, constructor.Rhs);
                }
            }

            return term;
        }

        public override bool Equals(Term? other) =>
            other is DiscriminatedUnionTerm rhs ? this.Constructors.SequenceEqual(rhs.Constructors) : false;
    }
}
