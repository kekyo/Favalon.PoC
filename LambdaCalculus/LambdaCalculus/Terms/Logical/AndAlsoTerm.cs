﻿using Favalon.TermContexts;

namespace Favalon.Terms.Logical
{
    public sealed class AndAlsoTerm : LogicalBinaryTerm<AndAlsoTerm>
    {
        private AndAlsoTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new AndAlsoTerm(lhs, rhs, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);
            if (lhs is BooleanTerm boolLhs)
            {
                if (!boolLhs.Value)
                {
                    return BooleanTerm.False;
                }
            }

            var rhs = this.Rhs.Reduce(context);
            if (rhs is BooleanTerm boolRhs)
            {
                return boolRhs;
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) &&
                this.Rhs.EqualsWithHigherOrder(rhs) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new AndAlsoTerm(lhs, rhs, higherOrder);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} && {this.Rhs.PrettyPrint(context)}";

        public static AndAlsoTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new AndAlsoTerm(lhs, rhs, higherOrder);
    }
}
