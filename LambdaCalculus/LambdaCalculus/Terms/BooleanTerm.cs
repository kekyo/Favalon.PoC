using Favalon.Terms.Types;
using LambdaCalculus.Contexts;

namespace Favalon.Terms
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

        public static new readonly ClrTypeTerm Type =
            (ClrTypeTerm)Type<bool>();

        public static new readonly BooleanTerm True =
            new BooleanTerm(true);
        public static new readonly BooleanTerm False =
            new BooleanTerm(false);

        public static BooleanTerm From(bool value) =>
            value ? True : False;
    }
}
