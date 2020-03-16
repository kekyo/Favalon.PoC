namespace Favalon.Terms
{
    public sealed class ConstantTerm :
        ValueTerm
    {
        internal ConstantTerm(object constant) =>
            this.Constant = constant;

        public override object Constant { get; }

        protected internal override string VisitTermString(bool includeTermName) =>
            this.Constant is string stringValue ?
                "\"" + stringValue + "\"" :
            this.Constant?.ToString() ?? "null";
    }
}
