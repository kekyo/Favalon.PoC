namespace Favalon.Terms
{
    public abstract class CallableTerm : Term
    {
        public readonly Term Parameter;

        internal CallableTerm(Term parameter) =>
            this.Parameter = parameter;

        protected internal sealed override Term VisitReduce(Context context) =>
            this;

        protected internal abstract Term VisitCall(Context context, Term argument);

        public void Deconstruct(out Term parameter) =>
            parameter = this.Parameter;
    }
}
