using System;

namespace Favalon.Expressions
{
    public sealed class BoundExpression :
        VariableExpression<BoundExpression>
    {
        public BoundExpression(string symbolName, Expression higherOrder)
        {
            this.SymbolName = symbolName;
            this.HigherOrder = higherOrder;
        }

        public override string SymbolName { get; }

        public override Expression HigherOrder { get; }

        protected override Expression VisitResolve(IInferContext context) =>
            new BoundExpression(
                this.SymbolName,
                this.HigherOrder.VisitResolveCore(context));

        public override bool Equals(Expression? rhs) =>
            base.Equals(rhs) &&
            rhs is BoundExpression variable ?
                this.SymbolName.Equals(variable.SymbolName) :
                false;
    }
}
