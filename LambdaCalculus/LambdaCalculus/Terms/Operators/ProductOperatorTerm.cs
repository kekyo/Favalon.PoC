using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public sealed class ProductOperatorTerm : BinaryOperatorTerm<ProductOperatorTerm>
    {
        private ProductOperatorTerm(Term higherOrder) :
            base(higherOrder)
        { }

        protected override Term OnCreate(Term higherOrder) =>
            new ProductOperatorTerm(higherOrder);

        protected override Term OnCreateClosure(Term lhs, Term higherOrder) =>
            new ProductOperatorClosureTerm(lhs, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "*";

        public static ProductOperatorTerm Create(Term higherOrder) =>
            new ProductOperatorTerm(LambdaTerm.Repeat(higherOrder, 3));

        private sealed class ProductOperatorClosureTerm : ClosureTerm<ProductOperatorClosureTerm>
        {
            public ProductOperatorClosureTerm(Term lhs, Term higherOrder) :
                base(lhs, higherOrder)
            { }

            protected override Term OnCreate(Term lhs, Term higherOrder) =>
                new ProductOperatorClosureTerm(lhs, higherOrder);

            protected override Term OnCreateClosure(Term lhs, Term rhs, Term higherOrder) =>
                ProductTerm.Create(new[] { lhs, rhs }, higherOrder);

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"* {this.Lhs.PrettyPrint(context)}";
        }
    }
}
