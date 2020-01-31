using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public interface IValueTerm
    {
        object Value { get; }
    }

    public abstract class ValueTerm : Term, IValueTerm
    {
        protected ValueTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        public object Value =>
            this.GetValue();

        protected abstract object GetValue();

        protected abstract Term OnCreate(object value, Term higherOrder);

        public override sealed Term Infer(InferContext context)
        {
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(this.Value, higherOrder);
        }

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

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value.ToString();
    }

    public abstract class ValueTerm<T> : ValueTerm
        where T : ValueTerm
    {
        protected ValueTerm(Term higherOrder) :
            base(higherOrder)
        { }

        protected override sealed bool OnEquals(EqualsContext context, Term? other) =>
            other is T rhs ? this.Value.Equals(rhs.Value) : false;
    }
}
