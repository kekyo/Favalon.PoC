using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class BooleanTerm : ValueTerm<BooleanTerm>
    {
        public new readonly bool Value;

        private BooleanTerm(bool value, Term higherOrder) :
            base(higherOrder) =>
            this.Value = value;

        protected override object GetValue() =>
            this.Value;

        protected override Term OnCreate(object value, Term higherOrder) =>
            new BooleanTerm((bool)value, higherOrder);

        public void Deconstruct(out bool value) =>
            value = this.Value;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value ? "true" : "false";

        public static readonly Term Type =
            TermFactory.Identity("bool");   // TODO: misunderstanding overrided bool terms.

        public static BooleanTerm Create(bool value, Term higherOrder) =>
            new BooleanTerm(value, higherOrder);

        public static BooleanTerm From(bool value) =>
            value ? True : False;

        public static readonly BooleanTerm True =
            new BooleanTerm(true, Type);
        public static readonly BooleanTerm False =
            new BooleanTerm(false, Type);
    }
}
