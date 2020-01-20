using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class AndAlsoTerm : LogicalBinaryTerm<AndAlsoTerm>
    {
        private AndAlsoTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        protected override Term OnCreate(Term lhs, Term rhs) =>
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
                lhs.EqualsWithHigherOrder(this.Lhs) &&
                rhs.EqualsWithHigherOrder(this.Rhs) ?
                    this :
                    new AndAlsoTerm(lhs, rhs);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} && {this.Rhs.PrettyPrint(context)}";

        public static AndAlsoTerm Create(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);
    }
}
