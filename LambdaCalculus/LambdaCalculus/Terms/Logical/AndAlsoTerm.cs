using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class AndAlsoTerm : BinaryTerm<AndAlsoTerm>
    {
        private static readonly Term higherOrder =
            ValueTerm.From(typeof(bool));

        private AndAlsoTerm(Term lhs, Term rhs) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new AndAlsoTerm(lhs, rhs);

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

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) ?
                    this :
                    new AndAlsoTerm(lhs, rhs);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} && {this.Rhs.PrettyPrint(context)}";

        public static AndAlsoTerm Create(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);
    }
}
