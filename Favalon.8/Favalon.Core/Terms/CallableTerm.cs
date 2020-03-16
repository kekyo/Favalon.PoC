namespace Favalon.Terms
{
    public abstract class CallableTerm : Term
    {
        internal CallableTerm()
        { }

        public abstract BoundIdentityTerm Parameter { get; }

        protected internal sealed override Term VisitReduce(Context context) =>
            this;

        protected internal abstract Term VisitCall(Context context, Term argument);

        public void Deconstruct(out BoundIdentityTerm parameter) =>
            parameter = this.Parameter;
    }
}
