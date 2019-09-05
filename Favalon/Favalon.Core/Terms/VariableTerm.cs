using Favalon.Expressions;
using System.Linq;

namespace Favalon.Terms
{
    public class VariableTerm : Term
    {
        public readonly string SymbolName;

        public VariableTerm(string symbolName) =>
            this.SymbolName = symbolName;

        public override int GetHashCode() =>
            this.SymbolName.GetHashCode();

        public bool Equals(VariableTerm? rhs) =>
            rhs is VariableTerm term ?
                this.SymbolName.Equals(term.SymbolName) :
                false;

        public override bool Equals(Term? other) =>
            this.Equals(other as VariableTerm);

        protected override Expression VisitInfer(IInferContext context)
        {
            if (context.Lookup(this.SymbolName).FirstOrDefault() is Expression lookup)
            {
                return lookup;
            }

            var sns = this.SymbolName.Split(':');
            return sns.
                Reverse().
                Aggregate(
                    UnspecifiedExpression.Instance,
                    (higherOrder, symbolName) => new VariableExpression(symbolName, higherOrder));
        }

        public override string ToString() =>
            $"{this.GetType().Name}: {this.SymbolName}";
    }

    public class VariableTerm<TTerm> : VariableTerm
        where TTerm : VariableTerm<TTerm>
    {
        protected VariableTerm(string symbolName) :
            base(symbolName)
        { }

        public bool Equals(TTerm? rhs) =>
            rhs is TTerm term ? base.Equals(term) : false;
    }
}
