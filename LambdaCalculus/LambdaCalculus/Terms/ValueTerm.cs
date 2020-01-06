using Favalon.Contexts;
using Favalon.Terms.Types;
using LambdaCalculus.Contexts;
using System;

namespace Favalon.Terms
{
    public abstract class ValueTerm : Term
    {
        protected ValueTerm()
        { }

        public object Value =>
            this.GetValue();

        protected abstract object GetValue();

        public override sealed Term Infer(InferContext context) =>
            this;

        public override sealed Term Fixup(FixupContext context) =>
            this;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed int GetHashCode() =>
            this.Value.GetHashCode();

        public void Deconstruct(out object value) =>
            value = this.Value;

        public void Deconstruct(out object value, out Term higherOrder)
        {
            value = this.Value;
            higherOrder = this.HigherOrder;
        }

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value.ToString();

        public static Term Create(object value) =>
            value switch
            {
                bool boolValue => BooleanTerm.From(boolValue),
                Type typeValue => TypeTerm.From(typeValue),
                _ => new ConstantTerm(value)
            };
    }

    public abstract class ValueTerm<T> : ValueTerm
        where T : ValueTerm
    {
        protected ValueTerm()
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ? this.Value.Equals(rhs.Value) : false;
    }
}
