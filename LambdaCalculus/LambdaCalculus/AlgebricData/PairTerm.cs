namespace Favalon.AlgebricData
{
    public sealed class PairTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        internal PairTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public override Term HigherOrder =>
            new PairTerm(this.Lhs.HigherOrder, this.Rhs.HigherOrder);

        public override Term Reduce(ReduceContext context) =>
            new PairTerm(this.Lhs.Reduce(context), this.Rhs.Reduce(context));

        public override Term Infer(InferContext context) =>
            new PairTerm(this.Lhs.Infer(context), this.Rhs.Infer(context));

        public override Term Fixup(FixupContext context) =>
            new PairTerm(this.Lhs.Fixup(context), this.Rhs.Fixup(context));

        public override bool Equals(Term? other) =>
            other is PairTerm rhs ? (rhs.Lhs.Equals(this.Lhs) && rhs.Rhs.Equals(this.Rhs)) : false;

        public void Deconstruct(out Term lhs, out Term rhs)
        {
            lhs = this.Lhs;
            rhs = this.Rhs;
        }
    }
}
