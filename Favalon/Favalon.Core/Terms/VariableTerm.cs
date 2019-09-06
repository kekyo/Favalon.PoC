using Favalon.Expressions;
using System.Collections.Generic;
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

        private static Expression InternalVisit(IInferContext context, IEnumerable<string> symbolNameElements) =>
            symbolNameElements.
            Reverse().
            Aggregate(
                UnspecifiedExpression.Instance,
                (higherOrder, symbolName) =>
                    context.Lookup(symbolName).FirstOrDefault() is Expression lookup ?
                    lookup :
                    new VariableExpression(symbolName, higherOrder));

        protected override Expression VisitInfer(IInferContext context) =>
            InternalVisit(context, this.SymbolName.Split(':'));

        internal Expression VisitInferForBound(IInferContext context)
        {
            var sns = this.SymbolName.Split(':');
            var expression1 = InternalVisit(context, sns.Skip(1));
            return new BoundExpression(sns[0], expression1);
        }

        public override string ToString() =>
            $"{this.GetType().Name}: {this.SymbolName}";

        public void Deconstruct(out string symbolName) =>
            symbolName = this.SymbolName;
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
