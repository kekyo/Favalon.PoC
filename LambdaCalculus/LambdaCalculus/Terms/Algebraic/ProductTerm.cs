using Favalon.Contexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class ProductTerm : BinaryTerm<ProductTerm>
    {
        private ProductTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ProductTerm(lhs, rhs, higherOrder);

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
                    new ProductTerm(lhs, rhs, higherOrder);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} * {this.Rhs.PrettyPrint(context)}";

        public static ProductTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new ProductTerm(lhs, rhs, higherOrder);
    }
}
