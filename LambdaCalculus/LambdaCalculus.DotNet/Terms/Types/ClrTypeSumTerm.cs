using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;
using System;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeSumTerm : SumTerm
    {
        private ClrTypeSumTerm(Term lhs, Term rhs) :
            base(lhs, rhs, KindTerm.Instance)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ClrTypeSumTerm(lhs, rhs);

        internal static T InternalReduce<T>(
            ReduceContext context, Term lhs, Term rhs, Func<Term?, Term, T> applied)
        {
            var lhs_ = lhs.Reduce(context);
            var rhs_ = rhs.Reduce(context);

            if (ClrTypeCalculator.Instance.Widening(lhs_, rhs_) is Term term1)
            {
                return applied(term1, rhs_);
            }
            if (ClrTypeCalculator.Instance.Widening(rhs_, lhs_) is Term term2)
            {
                return applied(term2, rhs_);
            }

            return
                lhs.EqualsWithHigherOrder(lhs_) &&
                rhs.EqualsWithHigherOrder(rhs_) ?
                    applied(null, rhs_) :
                    applied(new ClrTypeSumTerm(lhs_, rhs_), rhs_);
        }

        public override Term Reduce(ReduceContext context) =>
            InternalReduce(context, this.Lhs, this.Rhs, (term, _) => term ?? this);

        public static ClrTypeSumTerm Create(Term lhs, Term rhs) =>
            new ClrTypeSumTerm(lhs, rhs);
    }
}
