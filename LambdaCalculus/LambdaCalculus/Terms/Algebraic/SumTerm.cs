using Favalon.Contexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class SumTerm : BinaryTerm<SumTerm>
    {
        private SumTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);
            var rhs = this.Rhs.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new SumTerm(lhs, rhs, higherOrder);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";

        public static SumTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder);
    }
}
