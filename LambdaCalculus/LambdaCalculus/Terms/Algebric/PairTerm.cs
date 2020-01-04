using Favalon.Contexts;
using LambdaCalculus.Contexts;

namespace Favalon.Terms.Algebric
{
    public sealed class PairTerm : HigherOrderLazyTerm
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        internal PairTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        protected override Term GetHigherOrder() =>
            new PairTerm(this.Lhs.HigherOrder, this.Rhs.HigherOrder);

        public override Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);
            
            return new PairTerm(lhs, rhs);
        }

        public override Term Fixup(FixupContext context)
        {
            var lhs = this.Lhs.Fixup(context);
            var rhs = this.Rhs.Fixup(context);

            return new PairTerm(lhs, rhs);
        }

        public override Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);
            var rhs = this.Rhs.Reduce(context);

            return new PairTerm(lhs, rhs);
        }

        public override bool Equals(Term? other) =>
            other is PairTerm rhs ? (rhs.Lhs.Equals(this.Lhs) && rhs.Rhs.Equals(this.Rhs)) : false;

        public override int GetHashCode() =>
            this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();

        public void Deconstruct(out Term lhs, out Term rhs)
        {
            lhs = this.Lhs;
            rhs = this.Rhs;
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"({this.Lhs.PrettyPrint(context)} {this.Rhs.PrettyPrint(context)})";
    }
}
