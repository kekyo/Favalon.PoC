using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class BooleanTerm : ValueTerm<BooleanTerm>
    {
        public new readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        protected override object GetValue() =>
            this.Value;

        public override Term HigherOrder =>
            Type;

        public void Deconstruct(out bool value) =>
            value = this.Value;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value ? "true" : "false";

        public static readonly Term Type =
            TermFactory.Identity("bool");   // TODO: misunderstanding overrided bool terms.

        public static readonly BooleanTerm True =
            new BooleanTerm(true);
        public static readonly BooleanTerm False =
            new BooleanTerm(false);

        public static BooleanTerm From(bool value) =>
            value ? True : False;
    }
}
