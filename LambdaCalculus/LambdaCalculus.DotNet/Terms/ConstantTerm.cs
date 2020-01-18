using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class ConstantTerm : ValueTerm<ConstantTerm>
    {
        private readonly object value;

        internal ConstantTerm(object value) =>
            this.value = value;

        public override Term HigherOrder =>
            ClrTermFactory.Constant(this.value.GetType());

        protected override object GetValue() =>
            this.value;

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            higherOrderDetail switch
            {
                HigherOrderDetails.None => false,
                HigherOrderDetails.Full => true,
                _ => this.Value is string || this.Value is char
            };

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value is string str ? $"\"{str}\"" :
            this.Value.ToString();
    }
}
