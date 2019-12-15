using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class AndAlsoTerm : LogicalBinaryOperatorTerm<AndAlsoTerm>
    {
        internal AndAlsoTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        protected override Term Create(Term lhs, Term rhs, Term higherOrder) =>
            new AndAlsoTerm(lhs, rhs);

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Reduce(context);
            if (lhs_ is BooleanTerm l)
            {
                if (l.Value)
                {
                    var rhs_ = rhs.Reduce(context);
                    if (rhs_ is BooleanTerm r)
                    {
                        return BooleanTerm.From(r.Value);
                    }
                    else
                    {
                        return new AndAlsoTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return BooleanTerm.False;
                }
            }
            else
            {
                return new AndAlsoTerm(lhs_, rhs);
            }
        }

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);
    }
}
