using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class OrElseTerm : BinaryTerm<OrElseTerm>
    {
        private static readonly Term higherOrder =
            ValueTerm.From(typeof(bool));

        private OrElseTerm(Term lhs, Term rhs) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new OrElseTerm(lhs, rhs);

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
                    new OrElseTerm(lhs, rhs);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} || {this.Rhs.PrettyPrint(context)}";

        public static OrElseTerm Create(Term lhs, Term rhs) =>
            new OrElseTerm(lhs, rhs);
    }
}
