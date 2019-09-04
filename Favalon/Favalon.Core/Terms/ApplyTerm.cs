using Favalon.Expressions;

namespace Favalon.Terms
{
    public sealed class ApplyTerm : VariableTerm<ApplyTerm>
    {
        public readonly Term Rhs;

        public ApplyTerm(string symbolName, Term rhs) :
            base(symbolName) =>
            this.Rhs = rhs;

        protected override Expression Visit(InferContext context) =>
            new ApplyExpression(
                context.Lookup(this.SymbolName),
                this.Visit(context, this.Rhs));

        public override string ToString() =>
            $"{this.GetType().Name}: {this.SymbolName} {this.Rhs}";
    }
}
