namespace Favalon.Terms
{
    public sealed class BooleanTerm :
        ValueTerm
    {
        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override object Constant =>
            this.Value;

        protected internal override string VisitTermString(bool includeTermName) =>
            this.Value.ToString();

        public static readonly BooleanTerm True = new BooleanTerm(true);
        public static readonly BooleanTerm False = new BooleanTerm(false);

        public static BooleanTerm FromConstant(bool value) =>
            value ? True : False;
    }
}
