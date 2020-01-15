using Favalon.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Types
{
    public sealed class SumTypeTerm : BinaryTerm<SumTypeTerm>, ITypeTerm
    {
        private static readonly Term higherOrder =
            ValueTerm.From(typeof(Type));

        private SumTypeTerm(Term lhs, Term rhs) :
            base(lhs, rhs, higherOrder)
        { }

        public bool IsAssignableFrom(ITypeTerm fromType) =>
            TypeCalculator.Widening(this, (Term)fromType) != null;

        public int CompareTo(ITypeTerm other) =>
            TypeCalculator.WideningComparer.Compare(this, other);

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new SumTypeTerm(lhs, rhs);

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
                    new SumTypeTerm(lhs, rhs);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";
    }
}
