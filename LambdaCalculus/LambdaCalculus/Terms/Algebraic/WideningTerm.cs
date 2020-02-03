﻿using Favalon.Terms.Contexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class WideningTerm : BinaryTerm<WideningTerm>
    {
        private readonly AlgebraicCalculator calculator;

        private WideningTerm(Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator) :
            base(lhs, rhs, higherOrder) =>
            this.calculator = calculator;

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new WideningTerm(lhs, rhs, higherOrder, calculator);

        internal static Term? Reduce(
            ReduceContext context, Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator)
        {
            var lhs_= lhs.Reduce(context);
            var rhs_ = rhs.Reduce(context);

            if (calculator.Widening(lhs_, rhs_) is Term widen)
            {
                return widen;
            }

            var higherOrder_ = higherOrder.Reduce(context);

            return
                lhs_.EqualsWithHigherOrder(lhs) &&
                rhs_.EqualsWithHigherOrder(rhs) &&
                higherOrder.EqualsWithHigherOrder(higherOrder) ?
                    null :
                    new WideningTerm(lhs, rhs, higherOrder, calculator);
        }

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs, this.HigherOrder, calculator) ?? this;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} :> {this.Rhs.PrettyPrint(context)}";

        public static WideningTerm Create(Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator) =>
            new WideningTerm(lhs, rhs, higherOrder, calculator);
    }
}
