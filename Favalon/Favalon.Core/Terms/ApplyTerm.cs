using Favalon.Expressions;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class ApplyTerm : VariableTerm<ApplyTerm>
    {
        public readonly Term Rhs;

        public ApplyTerm(string symbolName, Term rhs) :
            base(symbolName) =>
            this.Rhs = rhs;

        protected override Expression VisitInfer(IInferContext context)
        {
            switch (this.Rhs)
            {
                case App
            }

            return new ApplyExpression(
                context.Lookup(this.SymbolName).First(),  // TODO:
                this.Visit(context, this.Rhs));
        }

        public override string ToString() =>
            $"{this.GetType().Name}: {this.SymbolName} {this.Rhs}";
    }
}
