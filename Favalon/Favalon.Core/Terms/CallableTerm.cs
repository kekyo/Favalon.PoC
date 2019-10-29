namespace Favalon.Terms
{
    public abstract class CallableTerm : Term
    {
        public readonly Term Parameter;

        internal CallableTerm(Term parameter) =>
            this.Parameter = parameter;

        public override Term VisitReduce() =>
            this;

        public abstract Term Call(Term argument);

        public void Deconstruct(out Term parameter) =>
            parameter = this.Parameter;
    }
}
