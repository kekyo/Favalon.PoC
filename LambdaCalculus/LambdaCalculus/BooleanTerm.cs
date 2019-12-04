namespace LambdaCalculus
{
    public sealed class BooleanTerm : Term
    {
        internal static readonly ClrTypeTerm higherOrder =
            Type<bool>();

        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override Term HigherOrder =>
            higherOrder;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is BooleanTerm rhs ? this.Value.Equals(rhs.Value) : false;

        public void Deconstruct(out bool value) =>
            value = this.Value;

        public static new readonly BooleanTerm True =
            new BooleanTerm(true);
        public static new readonly BooleanTerm False =
            new BooleanTerm(false);
    }
}
