using Favalon.Contexts;
using LambdaCalculus.Contexts;

namespace Favalon.Terms.Algebric
{
    public abstract class SumTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        protected SumTerm(Term lhs, Term rhs, Term higherOrder)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(lhs.HigherOrder, higherOrder);
            context.Unify(rhs.HigherOrder, higherOrder);

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new SumTerm(lhs, rhs, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var lhs = this.Lhs.Fixup(context);
            var rhs = this.Rhs.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new SumTerm(lhs, rhs, higherOrder);
        }

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

        public override bool Equals(Term? other) =>
            other is SumTerm;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";
    }
}
