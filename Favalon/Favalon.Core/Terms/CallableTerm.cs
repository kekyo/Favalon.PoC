namespace Favalon.Terms
{
    public abstract class CallableTerm : Term
    {
        internal CallableTerm()
        { }

        public abstract IdentityTerm Parameter { get; }

        protected internal sealed override Term VisitReduce(Context context) =>
            this;

        protected internal abstract Term VisitCall(Context context, Term argument);

        public void Deconstruct(out IdentityTerm parameter) =>
            parameter = this.Parameter;
    }
}
