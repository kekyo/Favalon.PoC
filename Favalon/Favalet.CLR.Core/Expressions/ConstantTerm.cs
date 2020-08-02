using Favalet.Contexts;
using Favalet.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Favalet.Expressions
{
    public interface IConstantTerm : ITerm
    {
        object Value { get; }
    }

    public sealed class ConstantTerm :
        Expression, IConstantTerm
    {
        private readonly LazySlim<ITerm> higherOrder;

        public readonly object Value;

        [DebuggerStepThrough]
        private ConstantTerm(object value)
        {
            this.Value = value;
            this.higherOrder = LazySlim.Create(() =>
                TypeTerm.From(this.Value.GetType()));
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get => higherOrder.Value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IConstantTerm.Value
        {
            [DebuggerStepThrough]
            get => this.Value;
        }

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(IConstantTerm rhs) =>
            this.Value.Equals(rhs.Value);

        public override bool Equals(IExpression? other) =>
            other is IConstantTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        private string StringValue
        {
            [DebuggerStepThrough]
            get => this.Value switch
            {
                string value => $"\"{value}\"",
                _ => this.Value.ToString()
            };
        }

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("value", this.StringValue) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                this.StringValue);

        [DebuggerStepThrough]
        public static ConstantTerm From(object value) =>
            new ConstantTerm(value);
    }
}
