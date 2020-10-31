using Favalet.Contexts;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    internal sealed class BoundVariableReferenceTerm :
        BoundVariableTermBase, IIdentityTerm
    {
        [DebuggerStepThrough]
        private BoundVariableReferenceTerm(string symbol, IExpression higherOrder) :
            base(symbol, higherOrder)
        {
        }
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Symbol
        {
            [DebuggerStepThrough]
            get => this.Symbol;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IIdentityTerm.Identity
        {
            [DebuggerStepThrough]
            get => this.Symbol;
        }

        private protected override BoundVariableTermBase OnCreate(
            string symbol,
            IExpression higherOrder) =>
            new BoundVariableReferenceTerm(symbol, higherOrder);

        protected override IExpression Reduce(IReduceContext context)
        {
            var variables = context.LookupVariables(this.Symbol);

            if (variables.Length >= 1)
            {
                // Nearly overloaded variable.
                return context.Reduce(variables[0].Expression);
            }
            else
            {
                return this;
            }
        }

        [DebuggerStepThrough]
        public static BoundVariableReferenceTerm Create(IBoundVariableTerm bound) =>
            new BoundVariableReferenceTerm(bound.Symbol, bound.HigherOrder);
    }
}
