namespace Favalon.Terms
{
    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override Term HigherOrder =>
            Type;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is BooleanTerm rhs ? this.Value.Equals(rhs.Value) : false;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public void Deconstruct(out bool value) =>
            value = this.Value;

        public static new readonly ClrTypeTerm Type =
            Type<bool>();

        public static new readonly BooleanTerm True =
            new BooleanTerm(true);
        public static new readonly BooleanTerm False =
            new BooleanTerm(false);

        public static BooleanTerm From(bool value) =>
            value ? True : False;
    }
}
