namespace Favalon.Terms
{
    public abstract partial class Term
    {
        protected internal virtual Term VisitTransposeFunction(Context context) =>
            this;  // TODO: abstract

        protected internal abstract Term VisitReplace(string identity, Term replacement);

        protected internal abstract Term VisitReduce(Context context);

        protected abstract string VisitTermString(bool includeTermName);

        private string GetTermName()
        {
            var name = this.GetType().Name;
            return name.EndsWith("Term") ? name.Substring(0, name.Length - 4) : name;
        }

        public string ToString(bool includeTermName) =>
            includeTermName ?
                $"{(this.GetTermName())}({this.VisitTermString(true)})" :
                this.VisitTermString(false);

        public string Readable =>
            this.ToString(false);

        public string Strict =>
            this.ToString(true);

        public override string ToString() =>
            this.ToString(true);
    }
}
