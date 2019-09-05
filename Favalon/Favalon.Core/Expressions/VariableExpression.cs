using System;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : Expression
    {
        private Expression higherOrder;

        public readonly string SymbolName;

        public VariableExpression(string symbolName, Expression higherOrder)
        {
            this.SymbolName = symbolName;
            this.higherOrder = higherOrder;
        }

        public override Expression HigherOrder =>
            higherOrder;

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is VariableExpression variable ?
                (this.SymbolName.Equals(variable.SymbolName) && this.HigherOrder.Equals(variable.HigherOrder)) :
                false;

        public override string ToString() =>
            $"{this.SymbolName}:{this.HigherOrder}";
    }
}
