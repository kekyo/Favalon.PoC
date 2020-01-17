using Favalon.Contexts;
using System;

namespace Favalon.Terms.Types
{
    public sealed class SumTypeTerm : BinaryTerm<SumTypeTerm>
    {
        private static readonly Term higherOrder =
            ValueTerm.From(typeof(Type));

        private readonly TypeCalculator typeCalculator;

        private  SumTypeTerm(Term lhs, Term rhs, TypeCalculator typeCalculator) :
            base(lhs, rhs, higherOrder) =>
            this.typeCalculator = typeCalculator;

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            typeCalculator.Sum(lhs, rhs);

        public override Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);
            if (lhs is BooleanTerm boolLhs)
            {
                if (boolLhs.Value)
                {
                    return boolLhs;
                }
            }

            var rhs = this.Rhs.Reduce(context);
            if (rhs is BooleanTerm boolRhs)
            {
                return boolRhs;
            }

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) ?
                    this :
                    typeCalculator.Sum(lhs, rhs);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";

        public static SumTypeTerm Create(Term lhs, Term rhs, TypeCalculator typeCalculator) =>
            new SumTypeTerm(lhs, rhs, typeCalculator);
    }
}
