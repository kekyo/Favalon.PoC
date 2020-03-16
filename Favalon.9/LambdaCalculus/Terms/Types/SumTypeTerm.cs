using Favalon.Terms.Contexts;
using System.Linq;

namespace Favalon.Terms.Algebraic
{
    public sealed class SumTypeTerm : SumTerm, IApplicableTerm
    {
        private SumTypeTerm(Term[] terms, Term higherOrder) :
            base(terms, higherOrder)
        { }

        protected override Term OnCreate(Term[] terms, Term higherOrder) =>
            new SumTypeTerm(terms, higherOrder);

        AppliedResult IApplicableTerm.ReduceForApply(
            ReduceContext context, Term argument, Term appliedHigherOrderHint)
        {
            var argument_ = argument.Reduce(context);
            var argumentHigherOrder = argument_.HigherOrder;

            var higherOrder = this.HigherOrder.Reduce(context);
            var lambda = LambdaTerm.From(argumentHigherOrder, appliedHigherOrderHint);

            var widenedHigherOrder = context.calculator.Widen(higherOrder, lambda);

            switch (widenedHigherOrder)
            {
                case Term _:
                    return AppliedResult.Applied(
                        this.Terms.First(t => t.HigherOrder.Equals(widenedHigherOrder)),
                        argument_);
                default:
                    return AppliedResult.Ignored(
                        this, higherOrder);
            }
        }

        public static new SumTypeTerm Create(Term[] terms, Term higherOrder) =>
            new SumTypeTerm(terms, higherOrder);

        public static new Term From(Term[] terms, Term higherOrder) =>
            terms.Length switch
            {
                0 => EmptyTerm.Create(higherOrder),
                1 => terms[0],
                _ => new SumTypeTerm(terms, higherOrder)
            };
    }
}
