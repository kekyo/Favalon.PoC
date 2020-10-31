using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace Favalet.Expressions.Specialized
{
    internal sealed class ReinterpretTerm :
        Expression, ITerm
    {
        public readonly string Symbol;

        [DebuggerStepThrough]
        private ReinterpretTerm(string symbol, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Symbol = symbol;
        }

        public override IExpression HigherOrder { get; }

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(ReinterpretTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        public override bool Equals(IExpression? other) =>
            other is ReinterpretTerm rhs && this.Equals(rhs);

        protected override IExpression MakeRewritable(IMakeRewritableContext context) =>
            new ReinterpretTerm(
                this.Symbol,
                context.MakeRewritableHigherOrder(this.HigherOrder));

        protected override IExpression Infer(IInferContext context)
        {
            var higherOrder = context.Infer(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new ReinterpretTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Fixup(IFixupContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new ReinterpretTerm(this.Symbol, higherOrder);
            }
        }

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

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("symbol", this.Symbol) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                this.Symbol);

        [DebuggerStepThrough]
        public static ReinterpretTerm Create(IBoundVariableTerm bound) =>
            new ReinterpretTerm(bound.Symbol, bound.HigherOrder);
    }
}
