using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Favalet.Expressions.Algebraic;
using Favalet.Internal;

namespace Favalet.Expressions
{
    public interface IVariableTerm :
        IIdentityTerm
    {
    }
    
    public sealed class VariableTerm :
        Expression, IVariableTerm
    {
        public readonly string Symbol;

        [DebuggerStepThrough]
        private VariableTerm(string symbol, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Symbol = symbol;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Symbol
        {
            [DebuggerStepThrough]
            get => this.Symbol;
        }

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        public override bool Equals(IExpression? other) =>
            other is IIdentityTerm rhs && this.Equals(rhs);

        protected override IExpression MakeRewritable(IMakeRewritableContext context) =>
            new VariableTerm(
                this.Symbol,
                context.MakeRewritableHigherOrder(this.HigherOrder));
        
        protected override IExpression Infer(IInferContext context)
        {
            var higherOrder = context.Infer(this.HigherOrder);
            var variables = context.LookupVariables(this.Symbol);

            if (variables.Length >= 1)
            {
                var targets = variables.
                    Where(v => !object.ReferenceEquals(this, v.Expression)).
                    Select(v =>
                        (symbolHigherOrder: context.Infer(context.MakeRewritableHigherOrder(v.SymbolHigherOrder)), 
                         expression: context.Infer(context.MakeRewritable(v.Expression)))).
                    Memoize();

                if (targets.Length >= 1)
                {
                    var symbolHigherOrder = LogicalCalculator.ConstructExpressions(
                        targets.Select(v => v.symbolHigherOrder).Memoize(), OrExpression.Create)!;

                    var expressionHigherOrder = LogicalCalculator.ConstructExpressions(
                        targets.Select(v => v.expression.HigherOrder).Memoize(), OrExpression.Create)!;
               
                    context.Unify(symbolHigherOrder, higherOrder);
                    context.Unify(expressionHigherOrder, higherOrder);
                }
            }

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new VariableTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Fixup(IFixupContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);
            var variables = context.LookupVariables(this.Symbol);

            if (variables.Length >= 1)
            {
                var targets = variables.
                    Where(v => !object.ReferenceEquals(this, v.Expression)).
                    Select(v => context.Fixup(v.Expression)).
                    Memoize();

                if (targets.Length >= 1)
                {
                    var targetsHigherOrder = LogicalCalculator.ConstructExpressions(
                        targets.Select(v => v.HigherOrder).Memoize(),
                        OrExpression.Create)!;
                
                    var calculated = context.TypeCalculator.Compute(
                        AndExpression.Create(
                            higherOrder, targetsHigherOrder));

                    var filteredHigherOrder = targets.
                        Where(v => context.TypeCalculator.Equals(v.HigherOrder, calculated)).
                        Select(v => v.HigherOrder).
                        Memoize();
                    
                    if (filteredHigherOrder.Length >= 1)
                    {
                        // Apply only calculated higher order.
                        var result = LogicalCalculator.ConstructExpressions(
                            filteredHigherOrder,
                            OrExpression.Create)!;
                        return new VariableTerm(this.Symbol, result);
                    }
                }
            }

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new VariableTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            var variables = context.LookupVariables(this.Symbol);

            if (variables.Length >= 1)
            {
                var reduced = context.Reduce(variables[0].Expression);
                return context.TypeCalculator.Compute(reduced);
            }
            else
            {
                return this;
            }
        }

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("symbol", this.Symbol) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                this.Symbol);

        [DebuggerStepThrough]
        public static VariableTerm Create(string symbol, IExpression higherOrder) =>
            new VariableTerm(symbol, higherOrder);
        [DebuggerStepThrough]
        public static VariableTerm Create(string symbol) =>
            new VariableTerm(symbol, UnspecifiedTerm.Instance);
    }
}
