using Favalon.Terms.Contexts;
using System;

namespace Favalon.Terms.Algebraic
{
    public class SumTerm : AlgebraicTerm<SumTerm>
    {
        protected SumTerm(Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator) :
            base(lhs, rhs, calculator) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder, this.Calculator);

        internal static T InternalReduce<T>(
            ReduceContext context,
            Term lhs,
            Term rhs,
            Term higherOrder,
            Func<Term?, Term, T> applied,
            AlgebraicCalculator calculator,
            Func<Term, Term, Term, Term> onCreate)
        {
            var lhs_ = lhs.Reduce(context);
            var rhs_ = rhs.Reduce(context);

            if (calculator.Widening(lhs_, rhs_) is Term term1)
            {
                return applied(term1, rhs_);
            }
            if (calculator.Widening(rhs_, lhs_) is Term term2)
            {
                return applied(term2, rhs_);
            }

            var higherOrder_ = higherOrder.Reduce(context);

            return
                lhs.EqualsWithHigherOrder(lhs_) &&
                rhs.EqualsWithHigherOrder(rhs_) &&
                higherOrder.EqualsWithHigherOrder(higherOrder_) ?
                    applied(null, rhs_) :
                    applied(onCreate(lhs_, rhs_, higherOrder_), rhs_);
        }

        public override Term Reduce(ReduceContext context) =>
            InternalReduce(
                context,
                this.Lhs,
                this.Rhs,
                this.HigherOrder,
                (term, _) => term ?? this,
                this.Calculator,
                this.OnCreate);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";

        public static SumTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder, AlgebraicCalculator.Instance);
    }
}
