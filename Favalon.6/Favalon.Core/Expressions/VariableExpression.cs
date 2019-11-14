using System;

namespace Favalon.Expressions
{
    public abstract class VariableExpression<TExpression> :
        Expression
        where TExpression : VariableExpression<TExpression>
    {
        protected VariableExpression()
        { }

        public abstract string SymbolName { get; }

        public override bool Equals(Expression? rhs) =>
            rhs is TExpression variable ?
                this.HigherOrder.Equals(variable.HigherOrder) :
                false;

        public override string ToString() =>
            $"{this.SymbolName}:{this.HigherOrder}";
    }

    public sealed class VariableExpression :
        VariableExpression<VariableExpression>
    {
        public VariableExpression(string symbolName, Expression higherOrder)
        {
            this.SymbolName = symbolName;
            this.HigherOrder = higherOrder;
        }

        public override string SymbolName { get; }

        public override Expression HigherOrder { get; }

        protected override Expression VisitResolve(IInferContext context) =>
            new VariableExpression(
                this.SymbolName,
                this.HigherOrder.VisitResolveCore(context));

        public override bool Equals(Expression? rhs) =>
            base.Equals(rhs) &&
            rhs is VariableExpression variable ?
                this.SymbolName.Equals(variable.SymbolName) :
                false;
    }
}
