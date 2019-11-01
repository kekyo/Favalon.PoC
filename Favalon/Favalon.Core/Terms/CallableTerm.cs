namespace Favalon.Terms
{
    public abstract class CallableTerm : Term
    {
        public readonly Term Parameter;

        internal CallableTerm(Term parameter) =>
            this.Parameter = parameter;

        public sealed override Term VisitReduce(Context context) =>
            this;

        public abstract Term Call(Context context, Term argument);

        public void Deconstruct(out Term parameter) =>
            parameter = this.Parameter;
    }
}
