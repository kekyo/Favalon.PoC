using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class EqualTerm : BinaryTerm<EqualTerm>
    {
        internal EqualTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override Term HigherOrder =>
            BooleanTerm.Type;

        protected override Term Create(Term lhs, Term rhs, Term higherOrder) =>
            new EqualTerm(lhs, rhs);

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs) =>
            lhs.Reduce(context).Equals(rhs.Reduce(context)) ?
                BooleanTerm.True :
                BooleanTerm.False;

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);
    }
}
