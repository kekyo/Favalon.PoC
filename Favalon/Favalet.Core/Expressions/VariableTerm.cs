﻿using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace Favalet.Expressions
{
    public sealed class VariableTerm :
        Expression, IIdentityTerm
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

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var variables = context.LookupVariables(this);

            if (variables.Length >= 1)
            {
                // TODO: overloading
                if (!object.ReferenceEquals(this, variables[0].Expression))
                {
                    var inferred = context.Infer(variables[0].Expression);
                    var symbolHigherOrder = context.InferHigherOrder(variables[0].SymbolHigherOrder);

                    context.Unify(symbolHigherOrder, higherOrder);
                    context.Unify(inferred.HigherOrder, higherOrder);
                }
            }

            var variable =
                object.ReferenceEquals(this.HigherOrder, higherOrder) ?
                    this :
                    new VariableTerm(this.Symbol, higherOrder);
            var placeholder =
                context.CreatePlaceholder(PlaceholderOrderHints.VariableOrAbove);

            context.Unify(placeholder, variable);

            return placeholder;
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            if (context.Resolve(this.Symbol) is IExpression resolved)
            {
                return context.Fixup(resolved);
            }
            else
            {
                var higherOrder = context.Fixup(this.HigherOrder);

                if (object.ReferenceEquals(this.HigherOrder, higherOrder))
                {
                    return this;
                }
                else
                {
                    return new VariableTerm(this.Symbol, higherOrder);
                }
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            var variables = context.LookupVariables(this);

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
            new VariableTerm(symbol, UnspecifiedTerm.TypeInstance);
    }
}
