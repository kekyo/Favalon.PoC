﻿using Favalet.Contexts;
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
                context.MakeRewritableHigherOrder(this.HigherOrder, HigherOrderAttributes.None));
        
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

                var symbolHigherOrder = targets.
                    Skip(1).
                    Aggregate(
                        targets[0].symbolHigherOrder,
                        (agg, v) => OrExpression.Create(agg, v.symbolHigherOrder));
                var expression = targets.
                    Skip(1).
                    Aggregate(
                        targets[0].expression,
                        (agg, v) => OrExpression.Create(agg, v.expression));
               
                context.Unify(symbolHigherOrder, higherOrder, true);
                context.Unify(expression.HigherOrder, higherOrder, true);
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
                var expression = targets.
                    Skip(1).
                    Aggregate(
                        targets[0],
                        OrExpression.Create);
                
                var calculated = context.TypeCalculator.Compute(
                    AndExpression.Create(
                        higherOrder, expression.HigherOrder));
                var filtered = 
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
